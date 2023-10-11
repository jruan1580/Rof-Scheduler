using DatamartManagementService.Domain;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Test.Importer
{
    [TestFixture]
    public class RevenueSummaryImporterTest
    {
        [Test]
        public async Task ImportRevenueSummary_NoExecutionHistory()
        {
            var revenueSummaryRepo = new Mock<IRevenueByDateUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();
            var detailedRevenueRepo = new Mock<IRevenueFromServicesRetrievalRepository>();

            var detailedRevenues = new List<RofRevenueFromServicesCompletedByDate>()
            {
                EntityCreator.GetDbDetailedRevenue()
            };

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync((JobExecutionHistory)null);

            detailedRevenueRepo.Setup(d => d.GetDetailedRevenueUpUntilDate(It.IsAny<DateTime>()))
                .ReturnsAsync(detailedRevenues);

            revenueSummaryRepo.Setup(r => r.AddRevenue(It.IsAny<RofRevenueByDate>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var revenueSummaryImporter = new RevenueSummaryImporter(revenueSummaryRepo.Object, jobExecutionHistoryRepo.Object, detailedRevenueRepo.Object);

            await revenueSummaryImporter.ImportRevenueSummary();

            revenueSummaryRepo.Verify(r =>
                r.AddRevenue(It.Is<RofRevenueByDate>(rs =>
                    rs.RevenueDate == new DateTime(2023, 10, 9) &&
                    rs.RevenueMonth == 10 &&
                    rs.RevenueYear == 2023 &&
                    rs.GrossRevenue == 20 &&
                    rs.NetRevenuePostEmployeePay == 5)),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Revenue Summary" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }

        [Test]
        public async Task ImportRevenueSummary_HasExecutionHistory()
        {
            var revenueSummaryRepo = new Mock<IRevenueByDateUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();
            var detailedRevenueRepo = new Mock<IRevenueFromServicesRetrievalRepository>();

            var detailedRevenues = new List<RofRevenueFromServicesCompletedByDate>()
            {
                EntityCreator.GetDbDetailedRevenue()
            };

            var lastExecution = EntityCreator.GetDbJobExecutionHistoryRevenue();

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync(lastExecution);

            detailedRevenueRepo.Setup(d => d.GetDetailedRevenueBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(detailedRevenues);

            revenueSummaryRepo.Setup(r => r.AddRevenue(It.IsAny<RofRevenueByDate>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var revenueSummaryImporter = new RevenueSummaryImporter(revenueSummaryRepo.Object, jobExecutionHistoryRepo.Object, detailedRevenueRepo.Object);

            await revenueSummaryImporter.ImportRevenueSummary();

            revenueSummaryRepo.Verify(r =>
                r.AddRevenue(It.Is<RofRevenueByDate>(rs =>
                    rs.RevenueDate == new DateTime(2023, 10, 9) &&
                    rs.RevenueMonth == 10 &&
                    rs.RevenueYear == 2023 &&
                    rs.GrossRevenue == 20 &&
                    rs.NetRevenuePostEmployeePay == 5)),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Revenue Summary" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }
    }
}
