using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IRevenueSummaryImporter
    {
        Task ImportRevenueSummary();
    }

    public class RevenueSummaryImporter : DataImportHelper, IRevenueSummaryImporter
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

        private async Task<RofRevenueByDate> GetRofRevenueByDate(List<JobEvent> completedEvents)
        {
            var petServiceInfo = await GetPetServiceInfoAssociatedWithJobEvent(completedEvents);

            var totalGrossRevenue = petServiceInfo.Sum(petService => petService.Price);
            var totalNetRevenue = totalGrossRevenue - 
                (petServiceInfo.Sum(petService => petService.EmployeeRate));

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
    }
}
