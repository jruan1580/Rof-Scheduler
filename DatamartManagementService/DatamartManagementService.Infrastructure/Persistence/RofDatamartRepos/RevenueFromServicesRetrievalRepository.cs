using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos
{
    public interface IRevenueFromServicesRetrievalRepository
    {
        Task<List<RofRevenueFromServicesCompletedByDate>> GetDetailedRevenueBetweenDates(DateTime startDate, DateTime endDate);
        Task<List<RofRevenueFromServicesCompletedByDate>> GetDetailedRevenueUpUntilDate(DateTime date);
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

        public async Task<List<RofRevenueFromServicesCompletedByDate>> GetDetailedRevenueBetweenDates(DateTime startDate, DateTime endDate)
        {
            using var context = new RofDatamartContext();

            return await context.RofRevenueFromServicesCompletedByDate
                .Where(r => r.RevenueDate > startDate
                    && r.RevenueDate <= endDate)
                .ToListAsync();
        }

        public async Task<List<RofRevenueFromServicesCompletedByDate>> GetDetailedRevenueUpUntilDate(DateTime date)
        {
            using var context = new RofDatamartContext();

            return await context.RofRevenueFromServicesCompletedByDate
                .Where(r => r.RevenueDate <= date)
                .ToListAsync();
        }
    }
}
