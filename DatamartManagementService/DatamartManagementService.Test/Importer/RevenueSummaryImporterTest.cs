using DatamartManagementService.Domain;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
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
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var revenueSummaryRepo = new Mock<IRevenueByDateUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync((JobExecutionHistory)null);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesUpUntilDate(It.IsAny<DateTime>()))
               .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetAllPetServices())
                .ReturnsAsync(petServices);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            revenueSummaryRepo.Setup(r => r.AddRevenue(It.IsAny<RofRevenueByDate>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var revenueSummaryImporter = new RevenueSummaryImporter(rofSchedulerRepo.Object, revenueSummaryRepo.Object, jobExecutionHistoryRepo.Object);

            await revenueSummaryImporter.ImportRevenueSummary();

            revenueSummaryRepo.Verify(r =>
                r.AddRevenue(It.Is<RofRevenueByDate>(rs =>
                    rs.RevenueDate == DateTime.Today.AddDays(-1) &&
                    rs.RevenueMonth == 10 &&
                    rs.RevenueYear == 2023 &&
                    rs.GrossRevenue == 25 &&
                    rs.NetRevenuePostEmployeePay == 10)),
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
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var revenueSummaryRepo = new Mock<IRevenueByDateUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var lastExecution = EntityCreator.GetDbJobExecutionHistoryRevenueSummary();
            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync(lastExecution);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
               .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetAllPetServices())
                .ReturnsAsync(petServices);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            revenueSummaryRepo.Setup(r => r.AddRevenue(It.IsAny<RofRevenueByDate>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var revenueSummaryImporter = new RevenueSummaryImporter(rofSchedulerRepo.Object, revenueSummaryRepo.Object, jobExecutionHistoryRepo.Object);

            await revenueSummaryImporter.ImportRevenueSummary();

            revenueSummaryRepo.Verify(r =>
                r.AddRevenue(It.Is<RofRevenueByDate>(rs =>
                    rs.RevenueDate == DateTime.Today.AddDays(-1) &&
                    rs.RevenueMonth == 10 &&
                    rs.RevenueYear == 2023 &&
                    rs.GrossRevenue == 25 &&
                    rs.NetRevenuePostEmployeePay == 10)),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Revenue Summary" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }
    }
}
