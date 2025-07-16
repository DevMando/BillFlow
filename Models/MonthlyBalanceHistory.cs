namespace BillFlow.Models
{
    public class MonthlyBalanceHistory
    {
        public MonthlyBalanceHistory() { }

        public MonthlyBalanceHistory(Account account, int year, int month)
        {
            Account = account;
            AccountID = account.ID;
            Year = year;
            Month = month;
            StartingBalance = 0m;
            EndingBalance = 0m;
            TotalDeposits = 0m;
            TotalDebits = 0m;
        }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal TotalDeposits { get; set; } = 0m;
        public decimal TotalDebits { get; set; } = 0m;

        // Navigation properties
        public Account Account { get; set; }

        // Calculated properties
        public decimal NetChange => TotalDeposits - TotalDebits;
        
        public string GetDisplayMonth()
        {
            return new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }

        public void UpdateTotals(List<Transaction> monthTransactions)
        {
            TotalDeposits = monthTransactions
                .Where(t => t.IsCredit)
                .Sum(t => t.Amount);

            TotalDebits = monthTransactions
                .Where(t => t.IsDebit)
                .Sum(t => t.Amount);

            EndingBalance = StartingBalance + NetChange;
        }
    }
}