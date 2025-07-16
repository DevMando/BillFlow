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
            
            // Simplified approach: Let's be more permissive and generate instances
            // The BillGenerationService will handle duplicate prevention
            
            return Frequency switch
            {
                "Monthly" => true,
                "Quarterly" => month % 3 == 1, // Jan, Apr, Jul, Oct
                "SemiAnnual" => month == 1 || month == 7, // Jan, Jul  
                "Annual" => month == 1, // January only
                _ => true // Default case for any other frequency
            };
        }
        
        // New method: Check if bill should be visible on PayBills page regardless of generation status
        public bool ShouldShowForMonth(int year, int month)
        {
            if (IsPayedOff) return false;
            
            // Show monthly bills every month
            if (Frequency == "Monthly") return true;
            
            // Show quarterly bills every month, but they generate quarterly
            if (Frequency == "Quarterly") return true;
            
            // Show semi-annual bills every month, but they generate semi-annually
            if (Frequency == "SemiAnnual") return true;
            
            // Show annual bills every month, but they generate annually
            if (Frequency == "Annual") return true;
            
            return true; // Default: show all bills
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
