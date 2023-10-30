using DatamartManagementService.Domain.Importer;
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
    public class PayrollSummaryImporterTest
    {
        [Test]
        public async Task ImportPayrollSummary_NoExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var payrollSummaryRepo = new Mock<IPayrollUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var employee = EntityCreator.GetDbEmployee();
            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync((JobExecutionHistory)null);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesUpUntilDate(It.IsAny<DateTime>()))
               .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(employee);

            rofSchedulerRepo.Setup(r => r.GetAllPetServices())
                .ReturnsAsync(petServices);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            payrollSummaryRepo.Setup(p => p.AddEmployeePayroll(It.IsAny<List<EmployeePayroll>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var payrollSummaryImporter = new PayrollSummaryImporter(rofSchedulerRepo.Object, payrollSummaryRepo.Object, jobExecutionHistoryRepo.Object);

            await payrollSummaryImporter.ImportPayrollSummary();

            payrollSummaryRepo.Verify(d =>
                d.AddEmployeePayroll(It.Is<List<EmployeePayroll>>(ps =>
                    ps[0].FirstName == "John" &&
                    ps[0].LastName == "Doe" &&
                    ps[0].EmployeeTotalPay == 15  &&
                    ps[0].PayrollDate == DateTime.Today.AddDays(-1) &&
                    ps[0].PayrollMonth == DateTime.Today.Month && 
                    ps[0].PayrollYear == DateTime.Today.Year)),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Payroll Summary" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }

        [Test]
        public async Task ImportPayrollSummary_HasExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var payrollSummaryRepo = new Mock<IPayrollUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var lastExecution = EntityCreator.GetDbJobExecutionHistoryRevenueSummary();
            var employee = EntityCreator.GetDbEmployee();
            var petServices = new List<PetServices>()
            {
                EntityCreator.GetDbPetService()
            };

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync(lastExecution);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
               .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(employee);

            rofSchedulerRepo.Setup(r => r.GetAllPetServices())
                .ReturnsAsync(petServices);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            payrollSummaryRepo.Setup(p => p.AddEmployeePayroll(It.IsAny<List<EmployeePayroll>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var payrollSummaryImporter = new PayrollSummaryImporter(rofSchedulerRepo.Object, payrollSummaryRepo.Object, jobExecutionHistoryRepo.Object);

            await payrollSummaryImporter.ImportPayrollSummary();

            payrollSummaryRepo.Verify(d =>
                d.AddEmployeePayroll(It.Is<List<EmployeePayroll>>(ps =>
                    ps[0].FirstName == "John" &&
                    ps[0].LastName == "Doe" &&
                    ps[0].EmployeeTotalPay == 15 &&
                    ps[0].PayrollDate == DateTime.Today.AddDays(-1) &&
                    ps[0].PayrollMonth == DateTime.Today.Month &&
                    ps[0].PayrollYear == DateTime.Today.Year)),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Payroll Summary" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }
    }
}
