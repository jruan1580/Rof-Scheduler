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
    public class DetailedRevenueImporterTest
    {
        [Test]
        public async Task ImportRevenueData_NoExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var detailedRevenueRepo = new Mock<IRevenueFromServicesUpsertRepository>();
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

            detailedRevenueRepo.Setup(d => d.AddRevenueFromServices(It.IsAny<List<RofRevenueFromServicesCompletedByDate>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var detailedRevenueImporter = new DetailedRevenueImporter(rofSchedulerRepo.Object, detailedRevenueRepo.Object, jobExecutionHistoryRepo.Object);

            await detailedRevenueImporter.ImportRevenueData();

            detailedRevenueRepo.Verify(d => 
                d.AddRevenueFromServices(It.Is<List<RofRevenueFromServicesCompletedByDate>>(lr => 
                    lr[0].EmployeeId == 1 &&
                    lr[0].EmployeeFirstName == "John" && 
                    lr[0].EmployeeLastName == "Doe" && 
                    lr[0].EmployeePay == 15 &&
                    lr[0].PetServiceId == 1 && 
                    lr[0].PetServiceName == "Walking" && 
                    lr[0].PetServiceRate == 25 && 
                    lr[0].IsHolidayRate == false && 
                    lr[0].NetRevenuePostEmployeeCut == 10 && 
                    lr[0].RevenueDate == new DateTime(2023, 9, 16, 8, 30, 0))), 
            Times.Once);

            jobExecutionHistoryRepo.Verify(j => 
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j => 
                    j.JobType == "Revenue" &&
                    j.LastDatePulled == DateTime.Today)), 
            Times.Once);
        }

        [Test]
        public async Task ImportRevenueData_HasExecutionHistory()
        {
            var rofSchedulerRepo = new Mock<IRofSchedRepo>();
            var detailedRevenueRepo = new Mock<IRevenueFromServicesUpsertRepository>();
            var jobExecutionHistoryRepo = new Mock<IJobExecutionHistoryRepository>();

            var jobEvents = new List<JobEvent>()
            {
                EntityCreator.GetDbJobEvent()
            };

            var employee = EntityCreator.GetDbEmployee();
            var petService = EntityCreator.GetDbPetService();
            var lastExecution = EntityCreator.GetDbJobExecutionHistoryRevenue();
            var holiday = EntityCreator.GetDbHoliday();
            var holidayRate = EntityCreator.GetDbHolidayRates();

            jobExecutionHistoryRepo.Setup(j => j.GetJobExecutionHistoryByJobType(It.IsAny<string>()))
                .ReturnsAsync(lastExecution);

            rofSchedulerRepo.Setup(r => r.GetCompletedServicesBetweenDates(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(jobEvents);

            rofSchedulerRepo.Setup(r => r.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(employee);

            rofSchedulerRepo.Setup(r => r.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(petService);

            rofSchedulerRepo.Setup(r => r.CheckIfJobDateIsHoliday(It.IsAny<DateTime>()))
                .ReturnsAsync(holiday);

            rofSchedulerRepo.Setup(r => r.GetHolidayRateByPetServiceId(It.IsAny<short>()))
                .ReturnsAsync(holidayRate);

            detailedRevenueRepo.Setup(d => d.AddRevenueFromServices(It.IsAny<List<RofRevenueFromServicesCompletedByDate>>()))
                .Returns(Task.CompletedTask);

            jobExecutionHistoryRepo.Setup(j => j.AddJobExecutionHistory(It.IsAny<JobExecutionHistory>()))
                .Returns(Task.CompletedTask);

            var detailedRevenueImporter = new DetailedRevenueImporter(rofSchedulerRepo.Object, detailedRevenueRepo.Object, jobExecutionHistoryRepo.Object);

            await detailedRevenueImporter.ImportRevenueData();

            detailedRevenueRepo.Verify(d =>
                d.AddRevenueFromServices(It.Is<List<RofRevenueFromServicesCompletedByDate>>(lr => 
                    lr[0].EmployeeId == 1 &&
                    lr[0].EmployeeFirstName == "John" &&
                    lr[0].EmployeeLastName == "Doe" &&
                    lr[0].EmployeePay == 23 &&
                    lr[0].PetServiceId == 1 &&
                    lr[0].PetServiceName == "Walking" &&
                    lr[0].PetServiceRate == 25 &&
                    lr[0].IsHolidayRate == true &&
                    lr[0].NetRevenuePostEmployeeCut == 2 &&
                    lr[0].RevenueDate == new DateTime(2023, 9, 16, 8, 30, 0))),
            Times.Once);

            jobExecutionHistoryRepo.Verify(j =>
                j.AddJobExecutionHistory(It.Is<JobExecutionHistory>(j => 
                    j.JobType == "Revenue" &&
                    j.LastDatePulled == DateTime.Today)),
            Times.Once);
        }
    }
}
