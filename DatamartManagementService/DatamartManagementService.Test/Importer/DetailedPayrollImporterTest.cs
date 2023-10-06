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

namespace DatamartManagementService.Test.Importer
{
    [TestFixture]
    public class DetailedPayrollImporterTest
    {
        [Test]
        public async Task ImportPayrollData_NoExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var detailedPayrollRepo = new Mock<IPayrollDetailUpsertRepository>();
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

            detailedPayrollRepo.Setup(d => d.AddEmployeePayrollDetail(It.IsAny<List<EmployeePayrollDetail>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var detailedPayrollImporter = new DetailedPayrollImporter(rofSchedulerRepo.Object, detailedPayrollRepo.Object, jobExecutionHistoryRepo.Object);

            await detailedPayrollImporter.ImportPayrollData();

            detailedPayrollRepo.Verify(d =>
                d.AddEmployeePayrollDetail(It.Is<List<EmployeePayrollDetail>>(p =>
                    p[0].EmployeeId == 1 &&
                    p[0].FirstName == "John" &&
                    p[0].LastName == "Doe" &&
                    p[0].EmployeePayForService == 15 &&
                    p[0].PetServiceId == 1 &&
                    p[0].PetServiceName == "Walking" &&
                    p[0].ServiceDuration == 1 &&
                    p[0].ServiceDurationTimeUnit == "Hour" &&
                    p[0].IsHolidayPay == false &&
                    p[0].ServiceStartDateTime == new DateTime(2023, 9, 16, 7, 30, 0) &&
                    p[0].ServiceEndDateTime == new DateTime(2023, 9, 16, 8, 30, 0))),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Payroll" &&
                    j.LastDatePulled == DateTime.Today.AddDays(-1))),
            Times.Once);
        }

        [Test]
        public async Task ImportPayrollData_HasExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var detailedPayrollRepo = new Mock<IPayrollDetailUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var employee = EntityCreator.GetDbEmployee();
            var petService = EntityCreator.GetDbPetService();
            var lastExecution = EntityCreator.GetDbJobExecutionHistoryPayroll();

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync(lastExecution);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(employee);

            rofSchedulerRepo.Setup(r => r.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(petService);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync((Holidays)null);

            detailedPayrollRepo.Setup(d => d.AddEmployeePayrollDetail(It.IsAny<List<EmployeePayrollDetail>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var detailedPayrollImporter = new DetailedPayrollImporter(rofSchedulerRepo.Object, detailedPayrollRepo.Object, jobExecutionHistoryRepo.Object);

            await detailedPayrollImporter.ImportPayrollData();

            detailedPayrollRepo.Verify(d =>
                d.AddEmployeePayrollDetail(It.Is<List<EmployeePayrollDetail>>(p =>
                    p[0].EmployeeId == 1 &&
                    p[0].FirstName == "John" &&
                    p[0].LastName == "Doe" &&
                    p[0].EmployeePayForService == 15 &&
                    p[0].PetServiceId == 1 &&
                    p[0].PetServiceName == "Walking" &&
                    p[0].ServiceDuration == 1 &&
                    p[0].ServiceDurationTimeUnit == "Hour" &&
                    p[0].IsHolidayPay == false &&
                    p[0].ServiceStartDateTime == new DateTime(2023, 9, 16, 7, 30, 0) &&
                    p[0].ServiceEndDateTime == new DateTime(2023, 9, 16, 8, 30, 0))),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j =>
                    j.JobType == "Payroll" &&
                    j.LastDatePulled == DateTime.Today.AddDays(-1))),
            Times.Once);
        }
    }
}
