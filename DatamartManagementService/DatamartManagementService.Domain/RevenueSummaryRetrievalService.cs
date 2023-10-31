using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofDatamartModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class RevenueSummaryRetrievalService
    {
        private readonly IRevenueByDateRetrievalRepository _revenueByDateRetrievalRepo;

        public RevenueSummaryRetrievalService(IRevenueByDateRetrievalRepository revenueByDateRetrievalRepo)
        {
            _revenueByDateRetrievalRepo = revenueByDateRetrievalRepo;
        }

        public async Task<List<RofRevenueByDate>> GetRevenueBetweenDates(DateTime startDate, DateTime endDate)
        {
            var dbRevenue = await _revenueByDateRetrievalRepo.GetRevenueBetweenDates(startDate, endDate);

            var revenueBtwnDates = RofDatamartMappers.ToCoreRevenueSummary(dbRevenue);

            return revenueBtwnDates;
        }
    }
}
