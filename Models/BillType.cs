namespace BillFlow.Models
{
    public class BillType
    {
        public BillType(){}

        public BillType(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
