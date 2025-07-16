namespace BillFlow.Models
{
    public class BillInstance
    {
        public BillInstance() { }

        public BillInstance(Bill bill, int year, int month)
        {
            Bill = bill;
            BillID = bill.ID;
            Year = year;
            Month = month;
            AmountDue = bill.AmountDue;
            IsPaid = false;
            DueDate = CalculateDueDate(bill.DueDate, year, month);
            CreatedDate = DateTime.Now;
        }

        public int ID { get; set; }
        public int BillID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal AmountDue { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Bill Bill { get; set; }
        public List<Payment>? Payments { get; set; }

        private static DateTime CalculateDueDate(DateTime? originalDueDate, int year, int month)
        {
            if (originalDueDate == null)
                return new DateTime(year, month, 1);

            var day = originalDueDate.Value.Day;
            var daysInMonth = DateTime.DaysInMonth(year, month);
            
            // Handle cases where due day doesn't exist in target month (e.g., Feb 30)
            if (day > daysInMonth)
                day = daysInMonth;

            return new DateTime(year, month, day);
        }

        public string GetDisplayMonth()
        {
            return new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }
    }
}