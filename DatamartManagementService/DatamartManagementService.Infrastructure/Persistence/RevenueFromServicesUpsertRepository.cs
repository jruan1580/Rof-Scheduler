using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class RevenueFromServicesUpsertRepository
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
