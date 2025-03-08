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

        public int ID { get; set; }
        public Bill Bill { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
