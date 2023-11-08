using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofDatamartModels;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IRevenueSummaryRetrievalService
    {
        Task<List<RevenueSummaryPerPetService>> GetRevenueBetweenDatesByPetService(DateTime startDate, DateTime endDate);
    }

    public class RevenueSummaryRetrievalService : IRevenueSummaryRetrievalService
    {
        private readonly IRevenueByDateRetrievalRepository _revenueByDateRetrievalRepo;
        private readonly IRofSchedRepo _rofSchedRepo;

        public RevenueSummaryRetrievalService(IRevenueByDateRetrievalRepository revenueByDateRetrievalRepo,
            IRofSchedRepo rofSchedRepo)
        {
            _revenueByDateRetrievalRepo = revenueByDateRetrievalRepo;
            _rofSchedRepo = rofSchedRepo;
        }

        public async Task<List<RevenueSummaryPerPetService>> GetRevenueBetweenDatesByPetService(DateTime startDate, DateTime endDate)
        {
            var dbRevenue = await _revenueByDateRetrievalRepo.GetRevenueBetweenDates(startDate, endDate);

            var revenue = RofDatamartMappers.ToCoreRevenueSummary(dbRevenue);

            var revenueByDates = revenue.GroupBy(r => r.PetServiceId)
                .ToDictionary(r => r.Key, r => r.ToList());

            var revenuePerService = await GetListOfRevenueSummaryPerPetService(revenueByDates);

            return revenuePerService;
        }

        private async Task<List<RevenueSummaryPerPetService>> GetListOfRevenueSummaryPerPetService(Dictionary<short, List<RofRevenueByDate>> revenueByDates)
        {
            var revenuePerService = new List<RevenueSummaryPerPetService>();
            
            var dbPetServices = await _rofSchedRepo.GetAllPetServices();
            var petServiceInfo = new PetServices();

            var petServices = dbPetServices.Select(dbService => RofSchedulerMappers.ToCorePetService(dbService))
                .ToDictionary(coreService => coreService.Id, coreService => coreService);

            foreach (var petServiceToRevenue in revenueByDates)
            {
                petServiceInfo = petServices[petServiceToRevenue.Key];
                revenuePerService.Add(new RevenueSummaryPerPetService(
                    petServiceInfo,
                    petServiceToRevenue.Value.Count,
                    petServiceToRevenue.Value.Sum(g => g.GrossRevenue),
                    petServiceToRevenue.Value.Sum(n => n.NetRevenuePostEmployeePay)));
            }

            return revenuePerService;
        }
    }
}
