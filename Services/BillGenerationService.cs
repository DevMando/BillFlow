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

        public async Task<List<BillInstance>> GenerateBillInstancesForMonth(int year, int month, bool forceGenerate = false)
        {
            var bills = await _context.Bills
                .Where(b => !b.IsPayedOff)
                .ToListAsync();

            // If forceGenerate is true, generate for all bills regardless of frequency
            var eligibleBills = forceGenerate 
                ? bills.ToList() 
                : bills.Where(b => b.ShouldGenerateForMonth(year, month)).ToList();

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

                    // Update bill's last generated month only for proper frequency intervals
                    // or if we're force generating
                    if (forceGenerate || 
                        bill.Frequency == "Monthly" || 
                        (bill.Frequency == "Quarterly" && month % 3 == 1) ||
                        (bill.Frequency == "SemiAnnual" && (month == 1 || month == 7)) ||
                        (bill.Frequency == "Annual" && month == 1))
                    {
                        bill.LastGeneratedMonth = new DateTime(year, month, 1);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return generatedInstances;
        }

        public async Task<List<BillInstance>> GetBillInstancesForMonth(int year, int month, bool generateIfMissing = true)
        {
            // Always generate missing instances first if requested
            if (generateIfMissing)
            {
                await GenerateBillInstancesForMonth(year, month);
            }
            
            // Then get all instances for the month
            var instances = await _context.BillInstances
                .Include(bi => bi.Bill)
                .ThenInclude(b => b.BillTag)
                .Include(bi => bi.Payments)
                .Where(bi => bi.Year == year && bi.Month == month)
                .OrderBy(bi => bi.DueDate)
                .ToListAsync();

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
        
        // Add a method specifically for force generating all bills for a month
        public async Task<List<BillInstance>> ForceGenerateAllBillsForMonth(int year, int month)
        {
            return await GenerateBillInstancesForMonth(year, month, forceGenerate: true);
        }

        public async Task<string> GetDebugInfo(int year, int month)
        {
            var allBills = await _context.Bills.ToListAsync();
            var activeBills = allBills.Where(b => !b.IsPayedOff).ToList();
            var eligibleBills = activeBills.Where(b => b.ShouldGenerateForMonth(year, month)).ToList();
            var existingInstances = await _context.BillInstances
                .Where(bi => bi.Year == year && bi.Month == month)
                .ToListAsync();

            var debugInfo = $"Total Bills: {allBills.Count}, Active Bills: {activeBills.Count}, " +
                   $"Eligible for {year}/{month}: {eligibleBills.Count}, " +
                   $"Existing Instances: {existingInstances.Count}";
            
            // Add details about filtered bills
            var filteredBills = activeBills.Where(b => !b.ShouldGenerateForMonth(year, month)).ToList();
            if (filteredBills.Any())
            {
                debugInfo += $" | Filtered Bills: {string.Join(", ", filteredBills.Select(b => $"{b.Name}({b.Frequency})"))}";
            }
            
            return debugInfo;
        }
    }
}