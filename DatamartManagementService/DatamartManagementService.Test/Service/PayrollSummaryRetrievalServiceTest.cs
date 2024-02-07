using DatamartManagementService.Domain;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Test.Service
{
    [TestFixture]
    public class PayrollSummaryRetrievalServiceTest
    {
        [Test]
        public async Task GetPayrollSummary_NoPayrollSummary()
        {
            var payrollRetrievalRepo = new Mock<IPayrollRetrievalRepository>();

            payrollRetrievalRepo.Setup(p => 
                p.GetEmployeePayrollBetweenDatesByEmployee(
                        It.IsAny<string>(), 
                        It.IsAny<string>(), 
                        It.IsAny<DateTime>(), 
                        It.IsAny<DateTime>()))
                .ReturnsAsync(new List<EmployeePayroll>());

            var payrollSummaryService = new PayrollSummaryRetrievalService(payrollRetrievalRepo.Object);

            var results = await payrollSummaryService.GetPayrollSummary("Peter", "Piper", DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsEmpty(results.PayrollSummaryPerEmployee);
            Assert.AreEqual(0, results.TotalPages);
        }

        [Test]
        public async Task GetPayrollSummary_Success()
        {
            var payrollRetrievalRepo = new Mock<IPayrollRetrievalRepository>();

            var payrollSummary = new List<EmployeePayroll>()
            {
                EntityCreator.GetDbPayrollSummary()
            };

            payrollRetrievalRepo.Setup(p =>
                p.GetEmployeePayrollBetweenDatesByEmployee(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                .ReturnsAsync(payrollSummary);

            var payrollSummaryService = new PayrollSummaryRetrievalService(payrollRetrievalRepo.Object);

            var results = await payrollSummaryService.GetPayrollSummary("John", "Doe", DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsNotEmpty(results.PayrollSummaryPerEmployee);
            Assert.AreEqual(1, results.TotalPages);

            Assert.IsNotNull(results.PayrollSummaryPerEmployee[0]);
            Assert.AreEqual("John", results.PayrollSummaryPerEmployee[0].FirstName);
            Assert.AreEqual("Doe", results.PayrollSummaryPerEmployee[0].LastName);
            Assert.AreEqual(20, results.PayrollSummaryPerEmployee[0].TotalPay);
        }

        [Test]
        public async Task GetPayrollSummary_NoNameProvidedSuccess()
        {
            var payrollRetrievalRepo = new Mock<IPayrollRetrievalRepository>();

            var payrollSummary = new List<EmployeePayroll>()
            {
                EntityCreator.GetDbPayrollSummary()
            };

            payrollRetrievalRepo.Setup(p =>
                p.GetEmployeePayrollBetweenDatesByEmployee(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                .ReturnsAsync(payrollSummary);

            var payrollSummaryService = new PayrollSummaryRetrievalService(payrollRetrievalRepo.Object);

            var results = await payrollSummaryService.GetPayrollSummary("", "", DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsNotEmpty(results.PayrollSummaryPerEmployee);
            Assert.AreEqual(1, results.TotalPages);

            Assert.IsNotNull(results.PayrollSummaryPerEmployee[0]);
            Assert.AreEqual(20, results.PayrollSummaryPerEmployee[0].TotalPay);
        }
    }
}
