using DatamartManagementService.Domain;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Test.Importer
{
    [TestFixture]
    public class DetailedRevenueImporterTest
    {
        [Test]
        public async Task ImportRevenueData_NoExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var detailedRevenueRepo = new Mock<IRevenueFromServicesUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var employee = EntityCreator.GetDbEmployee();
            var petService = EntityCreator.GetDbPetService();

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync((JobExecutionHistory)null);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesUpUntilDate(It.IsAny<DateTime>()))
                .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(employee);

            rofSchedulerRepo.Setup(r => r.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(petService);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            //calculate revenue

            detailedRevenueRepo.Setup(d => d.AddRevenueFromServices(It.IsAny<List<RofRevenueFromServicesCompletedByDate>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var detailedRevenueImporter = new DetailedRevenueImporter(rofSchedulerRepo.Object, detailedRevenueRepo.Object, jobExecutionHistoryRepo.Object);

            await detailedRevenueImporter.ImportRevenueData();

            detailedRevenueRepo.Verify(d => d.AddRevenueFromServices(It.IsAny<List<RofRevenueFromServicesCompletedByDate>>()), Times.Once);
        }
    }
}
