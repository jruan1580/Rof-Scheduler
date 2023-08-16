using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportEmployeePayroll : AImportRevenuePayroll
    {
        private readonly IRofSchedRepo _rofSchedRepo;

        public async Task<List<EmployeePayroll>> PopulateListOfEmployeePayroll(List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            var employeePayrolls = new List<EmployeePayroll>();

            foreach (var employee in employeeIds)
            {
                var payroll = await PopulateEmployeePayroll(employee, startDate, endDate);
                employeePayrolls.Add(payroll);
            }

            return employeePayrolls;
        }

        public async Task<EmployeePayroll> PopulateEmployeePayroll(long employeeId, DateTime startDate, DateTime endDate)
        {
            var employeeInfo = await _rofSchedRepo.GetEmployeeById(employeeId);
            var employeeTotalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, startDate, endDate);

            var newEmployeePayroll = new EmployeePayroll()
            {
                EmployeeId = employeeInfo.Id,
                FirstName = employeeInfo.FirstName,
                LastName = employeeInfo.LastName,
                EmployeeTotalPay = employeeTotalPay,
                PayPeriodStartDate = startDate,
                PayPeriodEndDate = endDate
            };

            return newEmployeePayroll;
        }
    }
}
