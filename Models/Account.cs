namespace BillFlow.Models
{
    public class Account
    {
        public Account() { }

        public Account(string name, string type, decimal monthlyIncome = 0m)
        {
            Name = name;
            Type = type;
            MonthlyIncome = monthlyIncome;
            CurrentBalance = 0m;
            IsActive = true;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Checking, Savings, Credit
        public decimal CurrentBalance { get; set; } = 0m;
        public decimal MonthlyIncome { get; set; } = 0m;
        public string? AccountNumber { get; set; }
        public string? Institution { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public List<Transaction>? Transactions { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<MonthlyBalanceHistory>? BalanceHistory { get; set; }
    }
}