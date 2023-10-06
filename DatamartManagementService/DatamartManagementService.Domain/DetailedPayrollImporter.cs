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
    public interface IDetailedPayrollImporter
    {
        Task ImportPayrollData();
    }

    public class DetailedPayrollImporter : DetailedDataImporter, IDetailedPayrollImporter
    {
        private readonly IPayrollDetailUpsertRepository _payrollDetailUpsertRepo;

        public DetailedPayrollImporter(IRofSchedRepo rofSchedRepo,
            IPayrollDetailUpsertRepository payrollDetailUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        : base(rofSchedRepo, jobExecutionHistoryRepo)
        {
            _payrollDetailUpsertRepo = payrollDetailUpsertRepo;
        }

        public async Task ImportPayrollData()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("payroll");

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, DateTime.Today);

                var listOfPayrollDetails = await GetListOfEmployeePayrollDetails(completedEvents);

                var listOfDbPayrollDetails =
                    RofDatamartMappers.FromCoreEmployeePayrollDetail(listOfPayrollDetails);

                await _payrollDetailUpsertRepo.AddEmployeePayrollDetail(listOfDbPayrollDetails);

                await AddJobExecutionHistory("Payroll", DateTime.Today);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
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
                await UpdateToHolidayPayRate(petServiceInfo);
            }

            var payrollDetail = new EmployeePayrollDetail()
            {
                EmployeeId = employeeInfo.Id,
                FirstName = employeeInfo.FirstName,
                LastName = employeeInfo.LastName,
                EmployeePayForService = petServiceInfo.EmployeeRate,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                ServiceDuration = petServiceInfo.Duration,
                ServiceDurationTimeUnit = petServiceInfo.TimeUnit,
                IsHolidayPay = isHolidayRate,
                JobEventId = jobEvent.Id,
                ServiceStartDateTime = jobEvent.EventStartTime,
                ServiceEndDateTime = jobEvent.EventEndTime
            };

            return payrollDetail;
        }
    }
}
