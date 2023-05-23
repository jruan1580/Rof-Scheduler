using DatamartManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IRevenueByDateUpsertRepository
    {
        Task AddRevenue(List<RofRevenueByDate> newRevenueByDate);
        Task UpdateRevenue(RofRevenueByDate updateRevenueByDate);
    }

    public class RevenueByDateUpsertRepository : IRevenueByDateUpsertRepository
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
