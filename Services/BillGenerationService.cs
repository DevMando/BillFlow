using BillFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BillFlow.Services
{
    public class BillGenerationService
    {
        private readonly BillFlowContext _context;

        public BillGenerationService(BillFlowContext context)
        {
            _context = context;
        }

        public async Task<List<BillInstance>> GenerateBillInstancesForMonth(int year, int month)
        {
            // Get all bills first for debugging
            var allBills = await _context.Bills.ToListAsync();
            
            var bills = await _context.Bills
                .Where(b => !b.IsPayedOff)
                .ToListAsync();

            // Filter using the method that we can debug
            var eligibleBills = bills.Where(b => b.ShouldGenerateForMonth(year, month)).ToList();

            var generatedInstances = new List<BillInstance>();

            foreach (var bill in eligibleBills)
            {
                // Check if instance already exists
                var existingInstance = await _context.BillInstances
                    .FirstOrDefaultAsync(bi => bi.BillID == bill.ID && bi.Year == year && bi.Month == month);

                if (existingInstance == null)
                {
                    var instance = new BillInstance(bill, year, month);
                    _context.BillInstances.Add(instance);
                    generatedInstances.Add(instance);

                    // Update bill's last generated month
                    bill.LastGeneratedMonth = new DateTime(year, month, 1);
                }
            }

            await _context.SaveChangesAsync();
            return generatedInstances;
        }

        public async Task<List<BillInstance>> GetBillInstancesForMonth(int year, int month, bool generateIfMissing = true)
        {
            // First try to get existing instances
            var instances = await _context.BillInstances
                .Include(bi => bi.Bill)
                .ThenInclude(b => b.BillTag)
                .Include(bi => bi.Payments)
                .Where(bi => bi.Year == year && bi.Month == month)
                .OrderBy(bi => bi.DueDate)
                .ToListAsync();

            // If no instances exist and generateIfMissing is true, generate them
            if (!instances.Any() && generateIfMissing)
            {
                await GenerateBillInstancesForMonth(year, month);
                
                // Reload instances after generation
                instances = await _context.BillInstances
                    .Include(bi => bi.Bill)
                    .ThenInclude(b => b.BillTag)
                    .Include(bi => bi.Payments)
                    .Where(bi => bi.Year == year && bi.Month == month)
                    .OrderBy(bi => bi.DueDate)
                    .ToListAsync();
            }

            return instances;
        }

        public async Task<List<BillInstance>> GetCurrentMonthBillInstances()
        {
            var now = DateTime.Now;
            return await GetBillInstancesForMonth(now.Year, now.Month);
        }

        public async Task MarkBillInstanceAsPaid(int billInstanceId, decimal amountPaid)
        {
            var instance = await _context.BillInstances.FindAsync(billInstanceId);
            if (instance != null && amountPaid >= instance.AmountDue)
            {
                instance.IsPaid = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> GenerateMissingInstancesForActiveMonths()
        {
            var currentDate = DateTime.Now;
            var generated = false;

            // Generate for current month and next month
            for (int monthOffset = 0; monthOffset <= 1; monthOffset++)
            {
                var targetDate = currentDate.AddMonths(monthOffset);
                var instances = await GenerateBillInstancesForMonth(targetDate.Year, targetDate.Month);
                if (instances.Any())
                {
                    generated = true;
                }
            }

            return generated;
        }

        public async Task<string> GetDebugInfo(int year, int month)
        {
            var allBills = await _context.Bills.ToListAsync();
            var activeBills = allBills.Where(b => !b.IsPayedOff).ToList();
            var eligibleBills = activeBills.Where(b => b.ShouldGenerateForMonth(year, month)).ToList();
            var existingInstances = await _context.BillInstances
                .Where(bi => bi.Year == year && bi.Month == month)
                .ToListAsync();

            return $"Total Bills: {allBills.Count}, Active Bills: {activeBills.Count}, " +
                   $"Eligible for {year}/{month}: {eligibleBills.Count}, " +
                   $"Existing Instances: {existingInstances.Count}";
        }
    }
}