using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofDatamartModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class PayrollSummaryRetrievalService
    {
        private readonly IPayrollRetrievalRepository _payrollRetrievalRepo;

        public PayrollSummaryRetrievalService(IPayrollRetrievalRepository payrollRetrievalRepo)
        {
            _payrollRetrievalRepo = payrollRetrievalRepo;
        }

        public async Task<PayrollSummaryWithTotalPages> GetPayrollSummary(string firstName, string lastName, DateTime startDate, DateTime endDate)
        {
            var dbPayroll = await _payrollRetrievalRepo.GetEmployeePayrollBetweenDatesByEmployee(firstName, lastName, startDate, endDate);

            var payroll = RofDatamartMappers.ToCorePayrollSummary(dbPayroll);

            var payrollSummary = payroll.GroupBy(p => p.EmployeeId)
                .ToDictionary(p => p.Key, p => p.ToList());

            var payrollSummaryPerEmployee = GetListOfPayrollSummaryPerEmployee(payrollSummary);

            var payrollWithPages = GetPayrollByPages(payrollSummaryPerEmployee);

            return new PayrollSummaryWithTotalPages(payrollWithPages.Item1, payrollWithPages.Item2);
        }

        private List<PayrollSummaryPerEmployee> GetListOfPayrollSummaryPerEmployee(Dictionary<long, List<EmployeePayroll>> payrollSummary)
        {
            var payrollPerEmployee = new List<PayrollSummaryPerEmployee>();

            foreach (var payrollToEmployee in payrollSummary)
            {
                payrollPerEmployee.Add(new PayrollSummaryPerEmployee(
                    payrollToEmployee.Value.Select(p => p.FirstName).ToString(),
                    payrollToEmployee.Value.Select(p => p.LastName).ToString(),
                    payrollToEmployee.Value.Sum(p => p.EmployeeTotalPay)));
            }

            return payrollPerEmployee;
        }

        private (List<PayrollSummaryPerEmployee>, int) GetPayrollByPages(List<PayrollSummaryPerEmployee> payrollPerEmployee, int page = 1, int offset = 10)
        {
            var skip = (page - 1) * offset;

            var totalPages = GetTotalPages(payrollPerEmployee.Count, offset, page);

            var result = payrollPerEmployee.OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName)
                    .Skip(skip)
                    .Take(offset).ToList();

            return (result, totalPages);
        }

        private int GetTotalPages(int numOfRecords, int pageSize, int pageRequested)
        {
            var numOfPages = numOfRecords / pageSize;
            int numOfExtraRecords = numOfRecords % pageSize;
            int totalPages = ((numOfExtraRecords > 0) ? (numOfPages + 1) : numOfPages);

            if (pageRequested > totalPages)
            {
                throw new Exception("Page requested is more than total number of pages");
            }

            return totalPages;
        }
    }
}
