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

namespace DatamartManagementService.Test.Service
{
    [TestFixture]
    public class RevenueSummaryRetrievalServiceTest
    {
        [Test]
        public async Task GetRevenueBetweenDates_NoRevenueSummary()
        {
            var revenueByDateRetrievalRepo = new Mock<IRevenueByDateRetrievalRepository>();
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();

            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            revenueByDateRetrievalRepo.Setup(r => r.GetRevenueBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<RofRevenueByDate>());

            rofSchedulerRepo.Setup(s => s.GetAllPetServices())
                .ReturnsAsync(petServices);

            var revenueSummaryService = new RevenueSummaryRetrievalService(revenueByDateRetrievalRepo.Object, rofSchedulerRepo.Object);

            var results = await revenueSummaryService.GetRevenueBetweenDatesByPetService(DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsEmpty(results);
        }

        [Test]
        public async Task GetRevenueBetweenDates_Success()
        {
            var revenueByDateRetrievalRepo = new Mock<IRevenueByDateRetrievalRepository>();
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();

            var revenueSummary = new List<RofRevenueByDate>()
            {
                EntityCreator.GetDbRevenueSummary()
            };

            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            revenueByDateRetrievalRepo.Setup(r => r.GetRevenueBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(revenueSummary);

            rofSchedulerRepo.Setup(s => s.GetAllPetServices())
                .ReturnsAsync(petServices);

            var revenueSummaryService = new RevenueSummaryRetrievalService(revenueByDateRetrievalRepo.Object, rofSchedulerRepo.Object);

            var results = await revenueSummaryService.GetRevenueBetweenDatesByPetService(DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Walking", results[0].PetService.ServiceName);
            Assert.AreEqual(1, results[0].Count);
            Assert.AreEqual(20, results[0].GrossRevenuePerService);
            Assert.AreEqual(5, results[0].NetRevenuePerService);
        }
    }
}
