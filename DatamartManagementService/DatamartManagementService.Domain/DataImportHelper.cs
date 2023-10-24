using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class DataImportHelper
    {
        protected readonly IRofSchedRepo _rofSchedRepo;
        protected readonly IJobExecutionHistoryRepository _jobExecutionHistoryRepo;

        public DataImportHelper(IRofSchedRepo rofSchedRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        {
            _rofSchedRepo = rofSchedRepo;
            _jobExecutionHistoryRepo = jobExecutionHistoryRepo;
        }

        protected async Task<JobExecutionHistory> GetJobExecutionHistory(string jobType)
        {
            var executionHistory = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType(jobType);

            if (executionHistory == null)
            {
                return null;
            }

            return RofDatamartMappers.ToCoreJobExecutionHistory(executionHistory);
        }

        protected async Task AddJobExecutionHistory(string jobType, DateTime lastDatePulled)
        {
            var newExecution = new JobExecutionHistory()
            {
                JobType = jobType,
                LastDatePulled = lastDatePulled
            };

            await _jobExecutionHistoryRepo.AddJobExecutionHistory(
                RofDatamartMappers.FromCoreJobExecutionHistory(newExecution));
        }

        protected async Task<List<JobEvent>> GetCompletedJobEventsBetweenDate(JobExecutionHistory jobExecution, DateTime endDate)
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

        protected async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate);

            return isHoliday != null;
        }

        protected async Task UpdateToHolidayPayRate(PetServices petService)
        {
            var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(
                    await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id));

            petService.EmployeeRate = holidayRate.HolidayRate;
        }
    }
}
