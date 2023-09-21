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
    public interface IDetailedRevenueImporter
    {
        Task ImportRevenueData();
    }

    public class DetailedRevenueImporter : ARevenueDataImporter, IDetailedRevenueImporter
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
            try
            {
                var lastExecution = await GetJobExecutionHistory();

                var yesterday = DateTime.Today.AddDays(-1);

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, yesterday);

                var listOfDetailedRofRev = await GetListOfRofRevenueOfCompletedServiceByDate(completedEvents);

                var revenueForServicesByDateDbEntity =
                    RofDatamartMappers.FromCoreRofRevenueFromServicesCompletedByDate(listOfDetailedRofRev);

                await _singleRevenueUpsertRepo.AddRevenueFromServices(revenueForServicesByDateDbEntity);

                await AddJobExecutionHistory("Revenue", yesterday);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }           
        }

        private async Task<JobExecutionHistory> GetJobExecutionHistory()
        {
            var executionHistory = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType("revenue");

            if(executionHistory == null)
            {
                return null;
            }

            return RofDatamartMappers.ToCoreJobExecutionHistory(executionHistory);
        }

        private async Task<List<JobEvent>> GetCompletedJobEventsBetweenDate(JobExecutionHistory jobExecution, DateTime endDate)
        {
            var jobEvents = new List<Infrastructure.Persistence.RofSchedulerEntities.JobEvent>();

            if (jobExecution == null)
            {
                jobEvents = await _rofSchedRepo.GetCompletedServicesUpUntilDate(endDate);
                return RofSchedulerMappers.ToCoreJobEvents(jobEvents);
            }

            jobEvents = await _rofSchedRepo.GetCompletedServicesBetweenDates(jobExecution.LastDatePulled, endDate);

            return RofSchedulerMappers.ToCoreJobEvents(jobEvents);
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

            var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(petServiceId));

            var isHolidayRate = await CheckIfHolidayRate(revenueDate);

            if (isHolidayRate)
            {
                await UpdateToHolidayPayRate(petServiceInfo);
            }

            var netRevenue = CalculateNetRevenueForCompletedService(petServiceInfo);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeeInfo.Id,
                EmployeeFirstName = employeeInfo.FirstName,
                EmployeeLastName = employeeInfo.LastName,
                EmployeePay = petServiceInfo.EmployeeRate,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                PetServiceRate = petServiceInfo.Price,
                IsHolidayRate = isHolidayRate,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = revenueDate
            };

            return rofRevenueForService;
        }

        private async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate);

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
