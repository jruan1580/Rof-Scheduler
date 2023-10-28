using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var employeeIdToJobEvents = jobEvents
                .GroupBy(jobs => jobs.EmployeeId)
                .ToDictionary(jobGroups => jobGroups.Key, jobGroups => jobGroups.ToList());

            foreach (var employee in employeeIdToJobEvents)
            {
                var jobsCompletedByEmployee = employee.Value; 
                var employeeInfo = await _rofSchedRepo.GetEmployeeById(employee.Key); 
                var petServiceInfo = await GetPetServiceInfoAssociatedWithJobEvent(employee.Value); 
                var totalPay = petServiceInfo.Sum(pet => pet.EmployeeRate);

                payrollSummary.Add(new EmployeePayroll()
                {
                    EmployeeId = employeeInfo.Id,
                    FirstName = employeeInfo.FirstName,
                    LastName = employeeInfo.LastName,
                    EmployeeTotalPay = totalPay,
                    PayrollDate = DateTime.Today.AddDays(-1),
                    PayrollMonth = Convert.ToInt16(DateTime.Today.Month),
                    PayrollYear = Convert.ToInt16(DateTime.Today.Year)
                });
            }

            return payrollSummary;
        }
    }
}
