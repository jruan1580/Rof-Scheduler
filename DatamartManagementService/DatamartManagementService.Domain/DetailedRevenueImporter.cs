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

    public class DetailedRevenueImporter : ARevenueDataImporter, IImportRofRevenueFromServicesCompletedByDate
    {
        private readonly IRevenueFromServicesUpsertRepository _singleRevenueUpsertRepo;
        private readonly IJobExecutionHistoryRepository _jobExecutionHistoryRepo;

        public DetailedRevenueImporter(IRofSchedRepo rofSchedRepo,
            IRevenueFromServicesUpsertRepository singleRevenueUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        : base(rofSchedRepo)
        {
            _singleRevenueUpsertRepo = singleRevenueUpsertRepo;
            _jobExecutionHistoryRepo = jobExecutionHistoryRepo;
        }

        public override async Task ImportRevenueData()
        {
            var executionHistory = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType("revenue");

            var lastExecution = RofDatamartMappers.ToCoreJobExecutionHistory(executionHistory);

            var yesterday = DateTime.Today.AddDays(-1);

            var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution.LastDatePulled, yesterday);

            var listOfDetailedRofRev = await GetListOfRofRevenueOfCompletedServiceByDate(completedEvents);

            var revenueForServicesByDateDbEntity = 
                RofDatamartMappers.FromCoreRofRevenueFromServicesCompletedByDate(listOfDetailedRofRev);

            await _singleRevenueUpsertRepo.AddRevenueFromServices(revenueForServicesByDateDbEntity);

            await AddJobExecutionHistory("Revenue", yesterday);
        }

        private async Task<List<JobEvent>> GetCompletedJobEventsBetweenDate(DateTime startDate, DateTime endDate)
        {
            if (startDate == null)
            {
                return RofSchedulerMappers.ToCoreJobEvents(
                    await _rofSchedRepo.GetCompletedServicesUpUntilDate(endDate));
            }

            return RofSchedulerMappers.ToCoreJobEvents(
                await _rofSchedRepo.GetCompletedServicesBetweenDates(startDate, endDate));
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> GetListOfRofRevenueOfCompletedServiceByDate(
            List<JobEvent> completedEvents)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var completed in completedEvents)
            {
                var revenue = await GetRofRevenueForServicesCompletedByDate(
                    completed.EmployeeId, 
                    completed.PetServiceId, 
                    completed.EventEndTime);

                revenueForServiceCompleted.Add(revenue);
            }
            
            return revenueForServiceCompleted;
        }

        private async Task<RofRevenueFromServicesCompletedByDate> GetRofRevenueForServicesCompletedByDate(
            long employeeId, 
            short petServiceId, 
            DateTime revenueDate)
        {           
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(employeeId));

            var petServiceInfo = await GetPetServiceWithCorrectPayRate(petServiceId, revenueDate);

            var isHolidayRate = await CheckIfHolidayRate(revenueDate);

            var netRevenue = CalculateNetRevenueForCompletedService(petServiceInfo);

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

        private async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = RofSchedulerMappers.ToCoreHoliday(
                await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate));

            return isHoliday != null;
        }

        private async Task AddJobExecutionHistory(string jobType, DateTime lastDatePulled)
        {
            var newExecution = new JobExecutionHistory()
            {
                JobType = jobType,
                LastDatePulled = lastDatePulled
            };

            await _jobExecutionHistoryRepo.AddJobExecutionHistory(
                RofDatamartMappers.FromCoreJobExecutionHistory(newExecution));
        }
    }
}
