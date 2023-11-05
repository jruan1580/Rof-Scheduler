using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofDatamartModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IRevenueSummaryRetrievalService
    {
        Task<Dictionary<short, List<RofRevenueByDate>>> GetRevenueBetweenDatesByPetService(DateTime startDate, DateTime endDate);
    }

    public class RevenueSummaryRetrievalService : IRevenueSummaryRetrievalService
    {
        private readonly IRevenueByDateRetrievalRepository _revenueByDateRetrievalRepo;

        public RevenueSummaryRetrievalService(IRevenueByDateRetrievalRepository revenueByDateRetrievalRepo)
        {
            _revenueByDateRetrievalRepo = revenueByDateRetrievalRepo;
        }

        public async Task<Dictionary<short, List<RofRevenueByDate>>> GetRevenueBetweenDatesByPetService(DateTime startDate, DateTime endDate)
        {
            var dbRevenue = await _revenueByDateRetrievalRepo.GetRevenueBetweenDates(startDate, endDate);

            var revenue = RofDatamartMappers.ToCoreRevenueSummary(dbRevenue);

            var revenueByDates = revenue.GroupBy(r => r.PetServiceId)
                .ToDictionary(r => r.Key, r => r.ToList());

            return revenueByDates;
        }
    }
}
