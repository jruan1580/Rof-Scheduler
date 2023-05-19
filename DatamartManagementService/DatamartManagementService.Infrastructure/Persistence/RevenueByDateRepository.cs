using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class RevenueByDateRepository
    {
        public async Task AddRevenue(List<RofRevenueByDate> newRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueByDate.AddRange(newRevenueByDate);

            await context.SaveChangesAsync();
        }

        public async Task<List<RofRevenueByDate>> GetRevenueForTheYear(short year)
        {
            using var context = new RofDatamartContext();

            var yearlyRevenue = await context.RofRevenueByDate.Where(r => r.RevenueYear == year).ToListAsync();

            return yearlyRevenue;
        }

        public async Task<List<RofRevenueByDate>> GetRevenueForTheMonthOfCertainYear(short month, short year)
        {
            using var context = new RofDatamartContext();

            var monthlyRevenue = await context.RofRevenueByDate.Where(r => r.RevenueMonth == month && r.RevenueYear == year).ToListAsync();

            return monthlyRevenue;
        }

        public async Task UpdateRevenue(RofRevenueByDate updateRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.Update(updateRevenueByDate);

            await context.SaveChangesAsync();
        }
    }
}
