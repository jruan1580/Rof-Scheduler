using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRevenueByDate : AImportRevenuePayroll
    {
        public ImportRevenueByDate(IRofSchedRepo rofSchedRepo) : base(rofSchedRepo) { }

        public async Task<List<RofRevenueByDate>> PopulateListOfRofRevenueByDate(List<long> employeeIds, DateTime revenueDate)
        {
            var revenueByDate = new List<RofRevenueByDate>();

            foreach (var employee in employeeIds)
            {
                var revenue = await PopulateRofRevenueByDate(employee, revenueDate);

                revenueByDate.Add(revenue);
            }

            return revenueByDate;
        }

        public async Task<RofRevenueByDate> PopulateRofRevenueByDate(long employeeId, DateTime revenueDate)
        {
            var grossRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, revenueDate, revenueDate);

            var netRevenue = await CalculateNetRevenueEarnedByDate(employeeId, revenueDate, revenueDate);

            var revenueByDate = new RofRevenueByDate()
            {
                RevenueDate = revenueDate,
                RevenueMonth = Convert.ToInt16(revenueDate.Month),
                RevenueYear = Convert.ToInt16(revenueDate.Year),
                GrossRevenue = grossRevenue,
                NetRevenuePostEmployeePay = netRevenue
            };

            return revenueByDate;
        }
    }
}
