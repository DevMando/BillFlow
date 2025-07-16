using BillFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BillFlow.Services
{
    public class PaymentProcessingService
    {
        private readonly BillFlowContext _context;

        public PaymentProcessingService(BillFlowContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessBillPayment(int billInstanceId, decimal amount, int accountId, int? secondaryAccountId = null, decimal? secondaryAmount = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get bill instance
                var billInstance = await _context.BillInstances
                    .Include(bi => bi.Bill)
                    .FirstOrDefaultAsync(bi => bi.ID == billInstanceId);

                if (billInstance == null)
                    throw new InvalidOperationException("Bill instance not found");

                // Get accounts
                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                    throw new InvalidOperationException("Primary account not found");

                Account? secondaryAccount = null;
                if (secondaryAccountId.HasValue)
                {
                    secondaryAccount = await _context.Accounts.FindAsync(secondaryAccountId.Value);
                    if (secondaryAccount == null)
                        throw new InvalidOperationException("Secondary account not found");
                }

                // Calculate total payment amount
                var totalAmount = amount + (secondaryAmount ?? 0);

                // Validate account balances
                if (account.CurrentBalance < amount)
                    throw new InvalidOperationException($"Insufficient funds in {account.Name}");

                if (secondaryAccount != null && secondaryAmount.HasValue && secondaryAccount.CurrentBalance < secondaryAmount.Value)
                    throw new InvalidOperationException($"Insufficient funds in {secondaryAccount.Name}");

                // Create payment record
                var payment = new Payment(billInstance, account, amount, secondaryAmount, secondaryAccount);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Create transactions for account debits
                var primaryTransaction = new Transaction(account, amount, TransactionTypes.Debit, $"Payment for {billInstance.Bill.Name}")
                {
                    PaymentID = payment.ID,
                    Year = billInstance.Year,
                    Month = billInstance.Month
                };
                _context.Transactions.Add(primaryTransaction);

                if (secondaryAccount != null && secondaryAmount.HasValue)
                {
                    var secondaryTransaction = new Transaction(secondaryAccount, secondaryAmount.Value, TransactionTypes.Debit, $"Payment for {billInstance.Bill.Name} (Split)")
                    {
                        PaymentID = payment.ID,
                        Year = billInstance.Year,
                        Month = billInstance.Month
                    };
                    _context.Transactions.Add(secondaryTransaction);
                }

                // Note: Account balances are managed manually by users and not automatically deducted

                // Update bill balance if it's a loan (has balance)
                if (billInstance.Bill.Balance > 0)
                {
                    billInstance.Bill.Balance -= totalAmount;
                    if (billInstance.Bill.Balance <= 0)
                    {
                        billInstance.Bill.Balance = 0;
                        billInstance.Bill.IsPayedOff = true;
                    }
                }

                // Mark bill instance as paid if fully paid
                if (totalAmount >= billInstance.AmountDue)
                {
                    billInstance.IsPaid = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Account>> GetAccountsWithBalances()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task ProcessMonthlyBalanceRefresh()
        {
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            // Get all active accounts
            var accounts = await _context.Accounts
                .Where(a => a.IsActive && a.MonthlyIncome > 0)
                .ToListAsync();

            foreach (var account in accounts)
            {
                // Check if we already processed this month
                var existingHistory = await _context.MonthlyBalanceHistory
                    .FirstOrDefaultAsync(mbh => mbh.AccountID == account.ID && mbh.Year == currentYear && mbh.Month == currentMonth);

                if (existingHistory == null)
                {
                    // Create history record for previous month
                    var previousMonth = currentDate.AddMonths(-1);
                    var prevMonthTransactions = await _context.Transactions
                        .Where(t => t.AccountID == account.ID && t.Year == previousMonth.Year && t.Month == previousMonth.Month)
                        .ToListAsync();

                    var history = new MonthlyBalanceHistory(account, previousMonth.Year, previousMonth.Month)
                    {
                        StartingBalance = account.CurrentBalance - account.MonthlyIncome, // Approximate
                        EndingBalance = account.CurrentBalance
                    };
                    history.UpdateTotals(prevMonthTransactions);
                    _context.MonthlyBalanceHistory.Add(history);

                    // Add monthly income to account
                    account.CurrentBalance += account.MonthlyIncome;

                    // Create deposit transaction
                    var depositTransaction = new Transaction(account, account.MonthlyIncome, TransactionTypes.MonthlyDeposit, "Monthly Income Deposit")
                    {
                        Year = currentYear,
                        Month = currentMonth
                    };
                    _context.Transactions.Add(depositTransaction);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalAccountBalance()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .SumAsync(a => a.CurrentBalance);
        }

        public async Task<List<Transaction>> GetAccountTransactionsForMonth(int accountId, int year, int month)
        {
            return await _context.Transactions
                .Where(t => t.AccountID == accountId && t.Year == year && t.Month == month)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> UnpayBill(int billInstanceId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get bill instance with its payments
                var billInstance = await _context.BillInstances
                    .Include(bi => bi.Bill)
                    .Include(bi => bi.Payments)
                    .ThenInclude(p => p.Account)
                    .Include(bi => bi.Payments)
                    .ThenInclude(p => p.SecondaryAccount)
                    .FirstOrDefaultAsync(bi => bi.ID == billInstanceId);

                if (billInstance == null)
                    throw new InvalidOperationException("Bill instance not found");

                if (!billInstance.IsPaid)
                    throw new InvalidOperationException("Bill is not paid");

                // Get all payments for this bill instance
                var payments = await _context.Payments
                    .Include(p => p.Account)
                    .Include(p => p.SecondaryAccount)
                    .Where(p => p.BillInstanceID == billInstanceId)
                    .ToListAsync();

                // Get all transactions for these payments
                var paymentIds = payments.Select(p => p.ID).ToList();
                var transactions = await _context.Transactions
                    .Where(t => paymentIds.Contains(t.PaymentID ?? 0))
                    .ToListAsync();

                // Note: Account balances are managed manually by users and not automatically adjusted

                // Reverse bill balance changes (for loans)
                if (billInstance.Bill.Balance >= 0)
                {
                    var totalPaidAmount = payments.Sum(p => p.TotalAmount);
                    billInstance.Bill.Balance += totalPaidAmount;
                    billInstance.Bill.IsPayedOff = false;
                }

                // Mark bill instance as unpaid
                billInstance.IsPaid = false;

                // Remove transactions
                _context.Transactions.RemoveRange(transactions);

                // Remove payments
                _context.Payments.RemoveRange(payments);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}