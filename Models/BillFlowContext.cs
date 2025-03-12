using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer;

namespace BillFlow.Models
{
    public class BillFlowContext : DbContext
    {
        public BillFlowContext() { }
        public BillFlowContext(DbContextOptions<BillFlowContext> options) : base(options) { }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BillTag> BillTags { get; set; }

    }
}
