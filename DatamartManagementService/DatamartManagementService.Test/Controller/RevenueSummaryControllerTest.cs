using DataMart.Controller;
using DatamartManagementService.Domain;
using DatamartManagementService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Test.Controller
{
    [TestFixture]
    public class RevenueSummaryControllerTest
    {
        [Test]
        public async Task GetRevenueBetweenDatesByPetService()
        {
            var revenueSummaryRetrievalService = new Mock<IRevenueSummaryRetrievalService>();

            var revSumPerPetService = new List<RevenueSummaryPerPetService>()
            {
                ModelCreator.GetCoreRevenueSummaryPerPetService()
            };

            revenueSummaryRetrievalService.Setup(c => c.GetRevenueBetweenDatesByPetService(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(revSumPerPetService);

            var controller = new RevenueSummaryController(revenueSummaryRetrievalService.Object);

            var response = await controller.GetRevenueBetweenDatesByPetService(DateTime.Today.AddDays(-1).ToString(), DateTime.Today.ToString());

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }
    }
}
