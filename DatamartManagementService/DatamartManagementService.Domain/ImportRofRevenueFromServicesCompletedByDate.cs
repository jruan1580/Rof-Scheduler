using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobEvent = DatamartManagementService.Domain.Models.RofSchedulerModels.JobEvent;

namespace DatamartManagementService.Domain
{
    public interface IImportRofRevenueFromServicesCompletedByDate
    {
        Task ImportRevenueData();
    }

    public class ImportRofRevenueFromServicesCompletedByDate : AImportRevenue, IImportRofRevenueFromServicesCompletedByDate
    {
        private readonly IRevenueFromServicesUpsertRepository _singleRevenueUpsertRepo;
        private readonly IJobExecutionHistoryRepository _jobExecutionHistoryRepo;

        public ImportRofRevenueFromServicesCompletedByDate(IRofSchedRepo rofSchedRepo,
            IRevenueFromServicesUpsertRepository singleRevenueUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
            : base(rofSchedRepo)
        {
            _singleRevenueUpsertRepo = singleRevenueUpsertRepo;
            _jobExecutionHistoryRepo = jobExecutionHistoryRepo;
        }

        public override async Task ImportRevenueData()
        {
            var lastExecution = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType("revenue");

            var yesterday = DateTime.Today.AddDays(-1);

            var completedEvents = await PullCompletedJobEventsBetweenDate(lastExecution.LastDatePulled, yesterday);

            var listOfDetailedRofRev = await PopulateListOfRofRevenueOfCompletedServiceByDate(completedEvents);
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(List<JobEvent> completedEvents)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var completed in completedEvents)
            {
                var revenue = await PopulateRofRevenueForServicesCompletedByDate(completed.EmployeeId, completed.PetServiceId, completed.EventEndTime);

                revenueForServiceCompleted.Add(revenue);
            }
            
            return revenueForServiceCompleted;
        }

        private async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(long employeeId, short petServiceId, DateTime revenueDate)
        {
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(employeeId));

            var petServiceInfo = await GetPetServiceWithCorrectPayRate(petServiceId, revenueDate);

            var isHolidayRate = await CheckIfHolidayRate(revenueDate);

            var netRevenue = await CalculateNetRevenueForCompletedService(petServiceInfo);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeeInfo.Id,
                EmployeeFirstName = employeeInfo.FirstName,
                EmployeeLastName = employeeInfo.LastName,
                EmployeePay = petServiceInfo.EmployeeRate,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                PetServiceRate = petServiceInfo.EmployeeRate,
                IsHolidayRate = isHolidayRate,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = revenueDate
            };

            return rofRevenueForService;
        }

        private async Task<List<JobEvent>> PullCompletedJobEventsBetweenDate(DateTime startDate, DateTime endDate)
        {
            if (startDate == null)
            {
                return RofSchedulerMappers.ToCoreJobEvents(
                    await _rofSchedRepo.GetCompletedServicesUpUntilDate(endDate));
            }

            return RofSchedulerMappers.ToCoreJobEvents(
                await _rofSchedRepo.GetCompletedServicesBetweenDates(startDate, endDate));
        }

        private async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = RofSchedulerMappers.ToCoreHoliday(
                await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate));

            return isHoliday != null;
        }
    }
}
