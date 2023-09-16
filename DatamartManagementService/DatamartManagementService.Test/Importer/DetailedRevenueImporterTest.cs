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

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync((JobExecutionHistory)null);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesUpUntilDate(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<JobEvent>());
        }
    }
}
