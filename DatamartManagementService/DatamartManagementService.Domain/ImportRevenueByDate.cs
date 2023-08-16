using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRevenueByDate : AImportRevenuePayroll
    {
        private readonly IRofSchedRepo _rofSchedRepo;

        public async Task<List<RofRevenueByDate>> PopulateListOfRofRevenueByDate(List<EmployeePayrollDetail> employees)
        {
            var revenueByDate = new List<RofRevenueByDate>();

            foreach (var employee in employees)
            {
                var revenue = await PopulateRofRevenueByDate(employee);

                revenueByDate.Add(revenue);
            }

            return revenueByDate;
        }

        public async Task<RofRevenueByDate> PopulateRofRevenueByDate(EmployeePayrollDetail employeePayrollDetail)
        {
            var grossRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeePayrollDetail.EmployeeId,
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var netRevenue = await CalculateNetRevenueEarnedByDate(employeePayrollDetail.EmployeeId,
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var revenueByDate = new RofRevenueByDate()
            {
                RevenueDate = employeePayrollDetail.ServiceEndDateTime,
                RevenueMonth = Convert.ToInt16(employeePayrollDetail.ServiceEndDateTime.Month),
                RevenueYear = Convert.ToInt16(employeePayrollDetail.ServiceEndDateTime.Year),
                GrossRevenue = grossRevenue,
                NetRevenuePostEmployeePay = netRevenue
            };

            return revenueByDate;
        }
    }
}
