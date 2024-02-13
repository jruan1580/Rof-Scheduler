using DataMart.Controller;
using DatamartManagementService.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DatamartManagementService.Test.Controller
{
    [TestFixture]
    public class PayrollSummaryControllerTest
    {
        [Test]
        public async Task GetPayrollBetweenDatesByEmployee()
        {
            var payrollSummaryRetrievalService = new Mock<IPayrollSummaryRetrievalService>();

            var payrollSumWithPage = ModelCreator.GetCorePayrollSummaryWithTotalPages();

            payrollSummaryRetrievalService.Setup(x => x.GetPayrollSummaryPerEmployeeByDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<int>()))
                    .ReturnsAsync(payrollSumWithPage);

            var controller = new PayrollSummaryController(payrollSummaryRetrievalService.Object);

            var payrollSummaryDTOGetRequest = DTOCreator.GetDTOPayrollSummaryGetRequest();

            var response = await controller.GetPayrollBetweenDatesByEmployee(payrollSummaryDTOGetRequest);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            var result = JsonConvert.SerializeObject(okObj.Value);

            Assert.AreEqual(okObj.StatusCode, 200);
            Assert.True(result.Contains("John"));
            Assert.True(result.Contains("Doe"));
            Assert.True(result.Contains("20"));
            Assert.True(result.Contains("1"));
        }
    }
}
