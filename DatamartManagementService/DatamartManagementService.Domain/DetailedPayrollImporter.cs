using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class DetailedPayrollImporter
    {
        private readonly IRofSchedRepo _rofSchedRepo;
        private readonly IPayrollDetailUpsertRepository _payrollDetailUpsertRepo;
        private readonly IJobExecutionHistoryRepository _jobExecutionHistoryRepo;

        public DetailedPayrollImporter(IRofSchedRepo rofSchedRepo,
            IPayrollDetailUpsertRepository payrollDetailUpsertRepo, 
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        {
            _rofSchedRepo = rofSchedRepo;
            _payrollDetailUpsertRepo = payrollDetailUpsertRepo;
            _jobExecutionHistoryRepo = jobExecutionHistoryRepo;
        }

        public async Task ImportPayrollData()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory();

                var yesterday = DateTime.Today.AddDays(-1);

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, yesterday);

                var listOfPayrollDetails = await GetListOfEmployeePayrollDetails(completedEvents);

                var listOfDbPayrollDetails =
                    RofDatamartMappers.FromCoreEmployeePayrollDetail(listOfPayrollDetails);

                await _payrollDetailUpsertRepo.AddEmployeePayrollDetail(listOfDbPayrollDetails);

                await AddJobExecutionHistory("Payroll", yesterday);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async Task<JobExecutionHistory> GetJobExecutionHistory()
        {
            var executionHistory = await _jobExecutionHistoryRepo.GetJobExecutionHistoryByJobType("revenue");

            if (executionHistory == null)
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

        private async Task<List<EmployeePayrollDetail>> GetListOfEmployeePayrollDetails(
            List<JobEvent> completedEvents)
        {
            var employeePayrollDetails = new List<EmployeePayrollDetail>();

            foreach (var completed in completedEvents)
            {
                var payroll = await GetEmployeePayrollDetail(completed);

                employeePayrollDetails.Add(payroll);
            }

            return employeePayrollDetails;
        }

        private async Task<EmployeePayrollDetail> GetEmployeePayrollDetail(JobEvent jobEvent)
        {
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(jobEvent.EmployeeId));

            var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(jobEvent.PetServiceId));

            var isHolidayRate = await CheckIfHolidayRate(jobEvent.EventEndTime);

            if (isHolidayRate)
            {
                //UpdateToHolidayPayRate
            }

            var employeePay = 0m;

            var payrollDetail = new EmployeePayrollDetail()
            {
                EmployeeId = employeeInfo.Id,
                FirstName = employeeInfo.FirstName,
                LastName = employeeInfo.LastName,
                EmployeePayForService = employeePay,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                ServiceDuration = petServiceInfo.Duration,
                ServiceDurationTimeUnit = petServiceInfo.TimeUnit,
                IsHolidayPay = isHolidayRate,
                JobEventId = 0,
                ServiceStartDateTime = jobEvent.EventStartTime,
                ServiceEndDateTime = jobEvent.EventEndTime
            };

            return payrollDetail;
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
