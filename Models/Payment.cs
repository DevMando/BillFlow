namespace BillFlow.Models
{
    public class Payment
    {
        public Payment() { }

        public Payment(Bill bill, decimal amount) 
        {
            Bill = bill;
            Amount = amount;
            Date = DateTime.Now;
        }

        public Payment(BillInstance billInstance, Account account, decimal amount, decimal? secondaryAmount = null, Account? secondaryAccount = null) 
        {
            BillInstance = billInstance;
            BillInstanceID = billInstance.ID;
            Bill = billInstance.Bill;
            Account = account;
            AccountID = account.ID;
            Amount = amount;
            SecondaryAmount = secondaryAmount;
            SecondaryAccount = secondaryAccount;
            SecondaryAccountID = secondaryAccount?.ID;
            Date = DateTime.Now;
        }

        public int ID { get; set; }
        public Bill Bill { get; set; }
        public int? BillInstanceID { get; set; }
        public int? AccountID { get; set; }
        public int? SecondaryAccountID { get; set; }
        public decimal Amount { get; set; }
        public decimal? SecondaryAmount { get; set; }
        public DateTime Date { get; set; }

        // Navigation properties
        public BillInstance? BillInstance { get; set; }
        public Account? Account { get; set; }
        public Account? SecondaryAccount { get; set; }
        public List<Transaction>? Transactions { get; set; }

        // Calculated properties
        public decimal TotalAmount => Amount + (SecondaryAmount ?? 0);
        public bool IsSplitPayment => SecondaryAmount.HasValue && SecondaryAmount > 0;
    }
}
