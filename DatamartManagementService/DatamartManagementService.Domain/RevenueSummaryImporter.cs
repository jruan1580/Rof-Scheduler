using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IRevenueSummaryImporter
    {
        Task ImportRevenueSummary();
    }

    public class RevenueSummaryImporter : IRevenueSummaryImporter
    {
        private readonly IRevenueByDateUpsertRepository _revenueSummaryUpsertRepo;
        private readonly IJobExecutionHistoryRepository _jobExecutionHistoryRepo;
        private readonly IRevenueFromServicesRetrievalRepository _detailedRevenueRetrievalRepo;

        public RevenueSummaryImporter(IRevenueByDateUpsertRepository revenueSummaryUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo,
            IRevenueFromServicesRetrievalRepository detailedRevenueRetrievalRepo)
        {
            _revenueSummaryUpsertRepo = revenueSummaryUpsertRepo;
            _jobExecutionHistoryRepo = jobExecutionHistoryRepo;
            _detailedRevenueRetrievalRepo = detailedRevenueRetrievalRepo;
        }

        public async Task ImportRevenueSummary()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("revenue summary");

                var detailedRevenues = await GetDetailedRevenueBetweenDates(lastExecution, DateTime.Today);

                var revenueSummary =
                    RofDatamartMappers.FromCoreRevenueSummary(GetRofRevenueByDate(detailedRevenues));

                await _revenueSummaryUpsertRepo.AddRevenue(revenueSummary);

                await AddJobExecutionHistory("Revenue Summary", DateTime.Today);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private RofRevenueByDate GetRofRevenueByDate(List<RofRevenueFromServicesCompletedByDate> detailedRevenues)
        {
            var totalGrossRevenue = CalculateTotalGrossRevenue(detailedRevenues);
            var totalNetRevenue = CalculateTotalNetRevenue(detailedRevenues);

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

        private async Task<List<RofRevenueFromServicesCompletedByDate>> GetDetailedRevenueBetweenDates(JobExecutionHistory jobExecution, DateTime endDate)
        {
            var detailedRevenue = new List<Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate>();

            if (jobExecution == null)
            {
                detailedRevenue = await _detailedRevenueRetrievalRepo.GetDetailedRevenueUpUntilDate(endDate);

                return RofDatamartMappers.ToCoreDetailedRevenue(detailedRevenue);
            }

            detailedRevenue = await _detailedRevenueRetrievalRepo.GetDetailedRevenueBetweenDates(jobExecution.LastDatePulled, endDate);

            return RofDatamartMappers.ToCoreDetailedRevenue(detailedRevenue);
        }

        private decimal CalculateTotalGrossRevenue(List<RofRevenueFromServicesCompletedByDate> detailedRevenues)
        {
            var totalGrossRevenue = 0m;

            foreach (var revenue in detailedRevenues)
            {
                totalGrossRevenue += revenue.PetServiceRate;
            }

            return totalGrossRevenue;
        }

        private decimal CalculateTotalNetRevenue(List<RofRevenueFromServicesCompletedByDate> detailedRevenues)
        {
            var totalNetRevenue = 0m;

            foreach (var revenue in detailedRevenues)
            {
                totalNetRevenue += revenue.NetRevenuePostEmployeeCut;
            }

            return totalNetRevenue;
        }

        private async Task<JobExecutionHistory> GetJobExecutionHistory(string jobType)
        {
            var executionHistory = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType(jobType);

            if (executionHistory == null)
            {
                return null;
            }

            return RofDatamartMappers.ToCoreJobExecutionHistory(executionHistory);
        }

        private async Task AddJobExecutionHistory(string jobType, DateTime lastDatePulled)
        {
            var newExecution = new JobExecutionHistory()
            {
                JobType = jobType,
                LastDatePulled = lastDatePulled
            };

            await _jobExecutionHistoryRepo.AddJobExecutionHistory(
                RofDatamartMappers.FromCoreJobExecutionHistory(newExecution));
        }
    }
}
