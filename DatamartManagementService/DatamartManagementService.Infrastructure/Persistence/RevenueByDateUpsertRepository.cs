using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class RevenueByDateUpsertRepository
    {
        public async Task AddRevenue(List<RofRevenueByDate> newRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueByDate.AddRange(newRevenueByDate);

            await context.SaveChangesAsync();
        }

        public async Task UpdateRevenue(RofRevenueByDate updateRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.Update(updateRevenueByDate);

            await context.SaveChangesAsync();
        }
    }
}
