using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofDatamartRepos
{
    public interface IRevenueFromServicesUpsertRepository
    {
        Task AddRevenueFromServices(List<RofRevenueFromServicesCompletedByDate> newRevenueFromServices);
        Task UpdateRevenueFromServices(RofRevenueFromServicesCompletedByDate updateRevenueFromServices);
    }

    public class RevenueFromServicesUpsertRepository : IRevenueFromServicesUpsertRepository
    {
        public async Task AddRevenueFromServices(List<RofRevenueFromServicesCompletedByDate> newRevenueFromServices)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueFromServicesCompletedByDate.AddRange(newRevenueFromServices);

            await context.SaveChangesAsync();
        }

        public async Task UpdateRevenueFromServices(RofRevenueFromServicesCompletedByDate updateRevenueFromServices)
        {
            using var context = new RofDatamartContext();

            context.Update(updateRevenueFromServices);

            await context.SaveChangesAsync();
        }
    }
}
