namespace BillFlow.Models
{
    public class Bill
    {
        public Bill() { }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; } = 0m;
        public decimal AmountDue { get; set; } = 0m;
        public decimal AmountPaid { get; set; } = 0m;
        public DateTime? DueDate { get; set; } = null; 
        public BillTag? BillTag { get; set; }
        public List<Payment>? Payments { get; set; } = null;
        public bool IsMonthy { get; set; } = false;
        public bool IsPayedOff { get; set; } = false;
    }
}
