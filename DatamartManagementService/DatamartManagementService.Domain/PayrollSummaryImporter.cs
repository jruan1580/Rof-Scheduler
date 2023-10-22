using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class PayrollSummaryImporter : DataImportHelper
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

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async Task<List<Employee>> GetUniqueEmployeeFromCompletedEvents(List<JobEvent> completedEvents)
        {
            var uniqueEmployees = new List<Employee>();

            for(int i = 0; i < completedEvents.Count; i++)
            {
                var dbEmployee = await _rofSchedRepo.GetEmployeeById(completedEvents[i].EmployeeId);

                var employee = RofSchedulerMappers.ToCoreEmployee(dbEmployee);

                if (!uniqueEmployees.Contains(employee))
                {
                    uniqueEmployees.Add(employee);
                }
            }

            return uniqueEmployees;
        }

        private async Task GetPayroll(List<Employee> employeeInfos, List<JobEvent> completedEvents, DateTime startDate, DateTime endDate)
        {
            for(int i = 0; i < employeeInfos.Count; i++)
            {
                var totalPay = 0m;

                var employeeEvents = completedEvents.Where(e => e.EmployeeId == employeeInfos[i].Id);

                foreach(var completed in employeeEvents)
                {
                    var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                        await _rofSchedRepo.GetPetServiceById(completed.PetServiceId));

                    var isHolidayRate = await CheckIfHolidayRate(completed.EventEndTime);

                    if (isHolidayRate)
                    {
                        await UpdateToHolidayPayRate(petServiceInfo);
                    }

                    totalPay += petServiceInfo.EmployeeRate;
                }

                var payrollSummary = new List<EmployeePayroll>();

                payrollSummary.Add(new EmployeePayroll()
                {
                    EmployeeId = employeeInfos[i].Id,
                    FirstName = employeeInfos[i].FirstName,
                    LastName = employeeInfos[i].LastName,
                    EmployeeTotalPay = totalPay,
                    PayPeriodStartDate = startDate,
                    PayPeriodEndDate = endDate
                });
            }
        }

        //public long Id { get; set; }
        //public long EmployeeId { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public decimal EmployeeTotalPay { get; set; }
        //public DateTime PayPeriodStartDate { get; set; }
        //public DateTime PayPeriodEndDate { get; set; }
    }
}
