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

            payrollRetrievalRepo.Setup(p => p.GetEmployeePayrollBetweenDatesByEmployee(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<EmployeePayroll>());

            var payrollSummaryService = new PayrollSummaryRetrievalService(payrollRetrievalRepo.Object);

            var results = await payrollSummaryService.GetPayrollSummary("Peter", "Piper", DateTime.Today.AddDays(-1), DateTime.Today);

            Assert.IsEmpty(results.PayrollSummaryPerEmployee);
        }
    }
}
