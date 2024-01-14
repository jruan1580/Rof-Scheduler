using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos
{
    public interface IPayrollRetrievalRepository
    {
        Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id);
        Task<(List<EmployeePayroll>, int)> GetEmployeePayrollBetweenDatesByEmployee(string firstName, string lastName, DateTime startDate, DateTime endDate);
    }

    public class PayrollRetrievalRepository : IPayrollRetrievalRepository
    {
        public async Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var employeePayroll = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id).ToListAsync();

            return employeePayroll;
        }

        public async Task<(List<EmployeePayroll>, int)> GetEmployeePayrollBetweenDatesByEmployee(string firstName, string lastName, 
            DateTime startDate, DateTime endDate)
        {
            using var context = new RofDatamartContext();

            var employeePayrollByDate = context.EmployeePayroll.Where(ep => ep.PayrollDate >= startDate
                && ep.PayrollDate <= endDate).AsQueryable();

            if(!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
            {
                employeePayrollByDate = FilterByEmployee(employeePayrollByDate, firstName, lastName);
            }

            return await GetPayrollByPages(employeePayrollByDate);
        }

        private IQueryable<EmployeePayroll> FilterByEmployee(IQueryable<EmployeePayroll> employeePayrollByDate, string firstName, string lastName)
        {
            if(!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
            {
                return employeePayrollByDate.Where(ep => ep.FirstName.ToLower().Contains(firstName)
                   || ep.LastName.ToLower().Contains(lastName));
            }

            return employeePayrollByDate.Where(ep => ep.FirstName == firstName
                && ep.LastName == lastName);
        }

        private async Task<(List<EmployeePayroll>, int)> GetPayrollByPages(IQueryable<EmployeePayroll> employeePayroll, int page = 1, int offset = 10)
        {
            var countByCriteria = await employeePayroll.CountAsync();

            var skip = (page - 1) * offset;

            var totalPages = GetTotalPages(countByCriteria, offset, page);

            var result = await employeePayroll.OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName)
                    .Skip(skip)
                    .Take(offset).ToListAsync();

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
