namespace BillFlow.Models
{
    public class BillTag
    {
        public BillTag(){}

        public BillTag(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
