namespace BillFlow.Models
{
    public class Bill
    {
        public Bill() { }
        
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Balance { get; set; } = 0m;
        public decimal AmountDue { get; set; } = 0m;
        public DateTime? DueDate { get; set; } = null; 
        public BillTag? BillTag { get; set; }
        public bool IsMonthy { get; set; } = false;
        public bool IsPayedOff { get; set; } = false;
        
        // New properties for recurring bills and generation tracking
        public string Frequency { get; set; } = "Monthly"; // Monthly, Quarterly, SemiAnnual, Annual
        public DateTime? NextDueDate { get; set; }
        public bool IsFixedAmount { get; set; } = true;
        public DateTime? LastGeneratedMonth { get; set; }
        
        // Navigation properties
        public List<Payment>? Payments { get; set; } = null;
        public List<BillInstance>? BillInstances { get; set; } = null;

        // Helper methods
        public bool ShouldGenerateForMonth(int year, int month)
        {
            if (IsPayedOff) return false;
            
            var targetDate = new DateTime(year, month, 1);
            
            // Check if we already generated for this month
            if (LastGeneratedMonth.HasValue && 
                LastGeneratedMonth.Value.Year == year && 
                LastGeneratedMonth.Value.Month == month)
                return false;

            return Frequency switch
            {
                "Monthly" => true,
                "Quarterly" => month % 3 == 1, // Jan, Apr, Jul, Oct
                "SemiAnnual" => month == 1 || month == 7, // Jan, Jul
                "Annual" => month == 1, // January only
                _ => true
            };
        }
    }

    public static class BillFrequency
    {
        public const string Monthly = "Monthly";
        public const string Quarterly = "Quarterly";
        public const string SemiAnnual = "SemiAnnual";
        public const string Annual = "Annual";
    }
}
