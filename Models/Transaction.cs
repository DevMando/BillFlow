namespace BillFlow.Models
{
    public class Transaction
    {
        public Transaction() { }

        public Transaction(Account account, decimal amount, string type, string description = "")
        {
            Account = account;
            AccountID = account.ID;
            Amount = amount;
            TransactionDate = DateTime.Now;
            Type = type;
            Description = description;
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
        }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; } // Debit, Credit, MonthlyDeposit
        public string? Description { get; set; }
        public int? PaymentID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        // Navigation properties
        public Account Account { get; set; }
        public Payment? Payment { get; set; }

        // Helper methods
        public bool IsDebit => Type == "Debit";
        public bool IsCredit => Type == "Credit" || Type == "MonthlyDeposit";

        public string GetDisplayMonth()
        {
            return new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }
    }

    public static class TransactionTypes
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
        public const string MonthlyDeposit = "MonthlyDeposit";
    }
}