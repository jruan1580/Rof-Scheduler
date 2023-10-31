using DatamartManagementService.Domain;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
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

            revenueByDateRetrievalRepo.Setup(r => r.GetRevenueBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<RofRevenueByDate>());

            var revenueSummaryService = new RevenueSummaryRetrievalService(revenueByDateRetrievalRepo.Object);

            var results = await revenueSummaryService.GetRevenueBetweenDates(DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsEmpty(results);
        }

        [Test]
        public async Task GetRevenueBetweenDates_Success()
        {
            var revenueByDateRetrievalRepo = new Mock<IRevenueByDateRetrievalRepository>();

            var revenueSummary = new List<RofRevenueByDate>()
            {
                EntityCreator.GetDbRevenueSummary()
            };

            revenueByDateRetrievalRepo.Setup(r => r.GetRevenueBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(revenueSummary);

            var revenueSummaryService = new RevenueSummaryRetrievalService(revenueByDateRetrievalRepo.Object);

            var results = await revenueSummaryService.GetRevenueBetweenDates(DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results[DateTime.Today].Count);
            Assert.AreEqual(DateTime.Today, results[DateTime.Today][0].RevenueDate);
            Assert.AreEqual(DateTime.Today.Month, results[DateTime.Today][0].RevenueMonth);
            Assert.AreEqual(DateTime.Today.Year, results[DateTime.Today][0].RevenueYear);
            Assert.AreEqual(20, results[DateTime.Today][0].GrossRevenue);
            Assert.AreEqual(5, results[DateTime.Today][0].NetRevenuePostEmployeePay);
        }
    }
}
