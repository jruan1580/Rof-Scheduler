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
    public interface IPayrollSummaryImporter
    {
        Task ImportPayrollSummary();
    }

    public class PayrollSummaryImporter : DataImportHelper, IPayrollSummaryImporter
    {
        private readonly IPayrollUpsertRepository _payrollSummaryUpsertRepo;

        public PayrollSummaryImporter(IRofSchedRepo rofSchedRepo,
            IPayrollUpsertRepository payrollSummaryUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        : base(rofSchedRepo, jobExecutionHistoryRepo)
        {
            _payrollSummaryUpsertRepo = payrollSummaryUpsertRepo;
        }

        public async Task ImportPayrollSummary()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("payroll summary");

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, DateTime.Today);

                var payrollSummary = await GetPayrollSummary(completedEvents);

                var dbPayrollSummary = RofDatamartMappers.FromCorePayrollSummary(payrollSummary);

                await _payrollSummaryUpsertRepo.AddEmployeePayroll(dbPayrollSummary);

                await AddJobExecutionHistory("Payroll Summary", DateTime.Today);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async Task<List<EmployeePayroll>> GetPayrollSummary(List<JobEvent> jobEvents)
        {
            var payrollSummary = new List<EmployeePayroll>();

            for (int i = 0; i < jobEvents.Count; i++)
            {
                var startDate = jobEvents[i].EventStartTime;
                var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                    await _rofSchedRepo.GetEmployeeById(jobEvents[i].EmployeeId));

                var totalPay = await CalculateTotalEmployeePay(jobEvents, i);

                i = totalPay.Item2;

                var endDate = jobEvents[i].EventEndTime;

                payrollSummary.Add(new EmployeePayroll()
                {
                    EmployeeId = employeeInfo.Id,
                    FirstName = employeeInfo.FirstName,
                    LastName = employeeInfo.LastName,
                    EmployeeTotalPay = totalPay.Item1,
                    PayPeriodStartDate = startDate,
                    PayPeriodEndDate = endDate
                });
            }

            return payrollSummary;
        }

        private async Task<(decimal, int)> CalculateTotalEmployeePay(List<JobEvent> jobEvents, int i)
        {
            var totalPay = 0m;

            var petServiceInfo = new PetServices();

            while (i != jobEvents.Count - 1
                    && jobEvents[i].EmployeeId == jobEvents[i + 1].EmployeeId)
            {
                petServiceInfo = await GetPetServiceInfo(jobEvents[i]);

                totalPay += petServiceInfo.EmployeeRate;

                i++;
            }

            petServiceInfo = await GetPetServiceInfo(jobEvents[i]);

            totalPay += petServiceInfo.EmployeeRate;

            return (totalPay, i);
        }

        private async Task<PetServices> GetPetServiceInfo(JobEvent jobEvent)
        {
            var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                        await _rofSchedRepo.GetPetServiceById(jobEvent.PetServiceId));

            var isHolidayRate = await CheckIfHolidayRate(jobEvent.EventEndTime);

            if (isHolidayRate)
            {
                await UpdateToHolidayPayRate(petServiceInfo);
            }

            return petServiceInfo;
        }
    }
}
