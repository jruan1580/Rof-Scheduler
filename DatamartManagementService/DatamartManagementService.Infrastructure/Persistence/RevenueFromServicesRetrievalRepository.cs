using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IRevenueFromServicesRetrievalRepository
    {
        Task<List<RofRevenueFromServicesCompletedByDate>> GetRevenueFromServicesByEmployee(long employeeId);
        Task<List<RofRevenueFromServicesCompletedByDate>> GetRevenueFromServicesByPetService(long petServiceId);
    }

    public class RevenueFromServicesRetrievalRepository : IRevenueFromServicesRetrievalRepository
    {
        public async Task<List<RofRevenueFromServicesCompletedByDate>> GetRevenueFromServicesByEmployee(long employeeId)
        {
            using var context = new RofDatamartContext();

            var employeeRevenue = await context.RofRevenueFromServicesCompletedByDate.Where(r => r.EmployeeId == employeeId).ToListAsync();

            return employeeRevenue;
        }

        public async Task<List<RofRevenueFromServicesCompletedByDate>> GetRevenueFromServicesByPetService(long petServiceId)
        {
            using var context = new RofDatamartContext();

            var petServiceRevenue = await context.RofRevenueFromServicesCompletedByDate.Where(r => r.PetServiceId == petServiceId).ToListAsync();

            return petServiceRevenue;
        }
    }
}
