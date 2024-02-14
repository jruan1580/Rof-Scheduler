using DataMart.Controller;
using DatamartManagementService.Domain;
using DatamartManagementService.DTO;
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
            Assert.That(typeof(OkObjectResult), Is.EqualTo(response.GetType()));

            var okObj = (OkObjectResult)response;

            var responseBody = okObj.Value;

            Assert.IsNotNull(responseBody);
            Assert.That(responseBody.GetType(), Is.EqualTo(typeof(PayrollSummaryWithTotalPagesDTO)));

            var result = (PayrollSummaryWithTotalPagesDTO)responseBody;

            Assert.That(okObj.StatusCode, Is.EqualTo(200));
            Assert.That(result.TotalPages, Is.EqualTo(1));
            Assert.That(result.PayrollSummaryPerEmployeeDTO[0].FirstName, Is.EqualTo("John"));
            Assert.That(result.PayrollSummaryPerEmployeeDTO[0].LastName, Is.EqualTo("Doe"));
            Assert.That(result.PayrollSummaryPerEmployeeDTO[0].TotalPay, Is.EqualTo(20));
        }
    }
}
