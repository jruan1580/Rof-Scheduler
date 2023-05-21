using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
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
