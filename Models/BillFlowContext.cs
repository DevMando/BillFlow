using Microsoft.EntityFrameworkCore;

namespace BillFlow.Models
{
    public class BillFlowContext : DbContext
    {
        public BillFlowContext() { }
        public BillFlowContext(DbContextOptions<BillFlowContext> options) : base(options) { }
        
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BillTag> BillTags { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BillInstance> BillInstances { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MonthlyBalanceHistory> MonthlyBalanceHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint for BillInstances
            modelBuilder.Entity<BillInstance>()
                .HasIndex(bi => new { bi.BillID, bi.Year, bi.Month })
                .IsUnique();

            // Configure unique constraint for MonthlyBalanceHistory
            modelBuilder.Entity<MonthlyBalanceHistory>()
                .HasIndex(mbh => new { mbh.AccountID, mbh.Year, mbh.Month })
                .IsUnique();

            // Configure decimal precision
            modelBuilder.Entity<Account>()
                .Property(a => a.CurrentBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Account>()
                .Property(a => a.MonthlyIncome)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bill>()
                .Property(b => b.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bill>()
                .Property(b => b.AmountDue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BillInstance>()
                .Property(bi => bi.AmountDue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.SecondaryAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyBalanceHistory>()
                .Property(mbh => mbh.StartingBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyBalanceHistory>()
                .Property(mbh => mbh.EndingBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyBalanceHistory>()
                .Property(mbh => mbh.TotalDeposits)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MonthlyBalanceHistory>()
                .Property(mbh => mbh.TotalDebits)
                .HasColumnType("decimal(18,2)");

            // Configure relationships
            modelBuilder.Entity<BillInstance>()
                .HasOne(bi => bi.Bill)
                .WithMany(b => b.BillInstances)
                .HasForeignKey(bi => bi.BillID);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.BillInstance)
                .WithMany(bi => bi.Payments)
                .HasForeignKey(p => p.BillInstanceID);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Account)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.AccountID);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountID);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payment)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PaymentID);

            modelBuilder.Entity<MonthlyBalanceHistory>()
                .HasOne(mbh => mbh.Account)
                .WithMany(a => a.BalanceHistory)
                .HasForeignKey(mbh => mbh.AccountID);
        }
    }
}
