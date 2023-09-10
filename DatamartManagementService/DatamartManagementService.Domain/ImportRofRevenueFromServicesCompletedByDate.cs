using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.RofDatamartRepos;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
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

            var completedEvents = await PullCompletedJobEventsByDate(lastExecution.LastDatePulled, yesterday);
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(
            List<long> employeeIds,
            List<short> petServiceIds, 
            DateTime revenueDate)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var employeeId in employeeIds)
            {
                foreach (var petServiceId in petServiceIds)
                {
                    var revenue = await PopulateRofRevenueForServicesCompletedByDate(employeeId, petServiceId, revenueDate);

                    revenueForServiceCompleted.Add(revenue);
                }
            }
            
            return revenueForServiceCompleted;
        }

        private async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(long employeeId, short petServiceId, DateTime revenueDate)
        {
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(employeeId));

            var petServiceInfo = await GetPetServiceWitPayRate(petServiceId, revenueDate);

            var completedEvents = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

            var completedPetServiceEventsForTheDay = await PullCompletedJobEventsOfAPetServiceForTheDay(completedEvents, petServiceId);

            var isHolidayRate = await CheckIfHolidayRate(revenueDate);

            var netRevenue = await CalculateTotalNetRevenueFromPetServiceComplete(completedPetServiceEventsForTheDay, revenueDate);

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

        private async Task<List<JobEvent>> PullCompletedJobEventsByDate(DateTime startDate, DateTime endDate)
        {
            var completedEvents = new List<JobEvent>();

            if (startDate == null)
            {
                completedEvents = RofSchedulerMappers.ToCoreJobEvents(
                    await _rofSchedRepo.GetCompletedServicesByEndDate(endDate));
            }
            else
            {
                completedEvents = RofSchedulerMappers.ToCoreJobEvents(
                    await _rofSchedRepo.GetCompletedServicesByDate(startDate, endDate));
            }

            return completedEvents;
        }

        private async Task<decimal> CalculateTotalNetRevenueFromPetServiceComplete(List<JobEvent> completedEvents, DateTime revenueDate)
        {
            decimal netRevenue = 0;

            foreach (var completed in completedEvents)
            {
                netRevenue += await CalculateNetRevenueEarnedByDate(completed.EmployeeId, revenueDate);
            }

            return netRevenue;
        }

        private async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = RofSchedulerMappers.ToCoreHoliday(
                await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate));

            var isHolidayRate = (isHoliday != null);

            return isHolidayRate;
        }
    }
}
