using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos
{
    public interface IRevenueByDateUpsertRepository
    {
        Task AddRevenue(RofRevenueByDate newRevenueByDate);
        Task UpdateRevenue(RofRevenueByDate updateRevenueByDate);
    }

    public class RevenueByDateUpsertRepository : IRevenueByDateUpsertRepository
    {
        public async Task AddRevenue(RofRevenueByDate newRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueByDate.Add(newRevenueByDate);

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
