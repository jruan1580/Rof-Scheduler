using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IRevenueSummaryImporter
    {
        Task ImportRevenueSummary();
    }

    public class RevenueSummaryImporter : DetailedDataImporter, IRevenueSummaryImporter
    {
        private readonly IRevenueByDateUpsertRepository _revenueSummaryUpsertRepo;

        public RevenueSummaryImporter(IRofSchedRepo rofSchedRepo,
            IRevenueByDateUpsertRepository revenueSummaryUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        : base(rofSchedRepo, jobExecutionHistoryRepo)
        {
            _revenueSummaryUpsertRepo = revenueSummaryUpsertRepo;
        }

        public async Task ImportRevenueSummary()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("revenue summary");

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, DateTime.Today);

                var revenueSummary = await GetRofRevenueByDate(completedEvents);
                    
                var dbRevSummary = RofDatamartMappers.FromCoreRevenueSummary(revenueSummary);

                await _revenueSummaryUpsertRepo.AddRevenue(dbRevSummary);

                await AddJobExecutionHistory("Revenue Summary", DateTime.Today);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async Task<RofRevenueByDate> GetRofRevenueByDate(List<JobEvent> jobEvents)
        {
            var petServiceInfo = await GetPetServiceInfo(jobEvents);

            var totalGrossRevenue = CalculateTotalGrossRevenue(petServiceInfo);
            var totalNetRevenue = CalculateTotalNetRevenue(petServiceInfo);

            var rofRevenue = new RofRevenueByDate()
            {
                RevenueDate = DateTime.Today.AddDays(-1),
                RevenueMonth = Convert.ToInt16(DateTime.Today.Month),
                RevenueYear = Convert.ToInt16(DateTime.Today.Year),
                GrossRevenue = totalGrossRevenue,
                NetRevenuePostEmployeePay = totalNetRevenue
            };

            return rofRevenue;
        }

        private async Task<List<PetServices>> GetPetServiceInfo(List<JobEvent> jobEvents)
        {
            var petServiceInfo = new List<PetServices>();

            foreach(var job in jobEvents)
            {
                var dbService = await _rofSchedRepo.GetPetServiceById(job.PetServiceId);

                var petService = RofSchedulerMappers.ToCorePetService(dbService);

                var isHolidayRate = await CheckIfHolidayRate(job.EventEndTime);

                if (isHolidayRate)
                {
                    await UpdateToHolidayPayRate(petService);
                }

                petServiceInfo.Add(petService);
            }

            return petServiceInfo;
        }

        private decimal CalculateTotalGrossRevenue(List<PetServices> petServices)
        {
            var totalGrossRevenue = 0m;

            foreach (var service in petServices)
            {
                totalGrossRevenue += service.Price;
            }

            return totalGrossRevenue;
        }

        private decimal CalculateTotalNetRevenue(List<PetServices> petServices)
        {
            var totalNetRevenue = 0m;

            foreach (var service in petServices)
            {
                var netRev = service.Price - service.EmployeeRate;
                totalNetRevenue += netRev;
            }

            return totalNetRevenue;
        }
    }
}
