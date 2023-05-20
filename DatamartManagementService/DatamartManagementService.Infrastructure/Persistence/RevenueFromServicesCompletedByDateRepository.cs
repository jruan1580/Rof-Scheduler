using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class RevenueFromServicesCompletedByDateRepository
    {
        public async Task AddRevenueFromServices(List<RofRevenueFromServicesCompletedByDate> newRevenueFromServices)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueFromServicesCompletedByDate.AddRange(newRevenueFromServices);

            await context.SaveChangesAsync();
        }

        public async Task<List<RofRevenueFromServicesCompletedByDate>> GetRevenueFromServicesByEmployee(long employeeId)
        {
            using var context = new RofDatamartContext();

            var employeeRevenue = await context.RofRevenueFromServicesCompletedByDate.Where(r => r.EmployeeId == employeeId).ToListAsync();

            return employeeRevenue;
        }

        public async Task<List<RofRevenueFromServicesCompletedByDate>> RevenueFromServicesByPetService(long petServiceId)
        {
            using var context = new RofDatamartContext();

            var petServiceRevenue = await context.RofRevenueFromServicesCompletedByDate.Where(r => r.PetServiceId == petServiceId).ToListAsync();

            return petServiceRevenue;
        }

        public async Task UpdateRevenueFromServices(RofRevenueFromServicesCompletedByDate updateRevenueFromServices)
        {
            using var context = new RofDatamartContext();

            context.Update(updateRevenueFromServices);

            await context.SaveChangesAsync();
        }
    }
}
