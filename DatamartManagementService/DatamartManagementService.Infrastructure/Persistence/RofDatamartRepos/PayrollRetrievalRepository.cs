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
        Task<List<EmployeePayroll>> GetEmployeePayrollBetweenDatesByEmployee(string firstName, string lastName, DateTime startDate, DateTime endDate);
    }

    public class PayrollRetrievalRepository : IPayrollRetrievalRepository
    {
        public async Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var employeePayroll = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id).ToListAsync();

            return employeePayroll;
        }

        public async Task<List<EmployeePayroll>> GetEmployeePayrollBetweenDatesByEmployee(string firstName, string lastName, 
            DateTime startDate, DateTime endDate)
        {
            using var context = new RofDatamartContext();

            var employeePayrollByDate = context.EmployeePayroll.Where(ep => ep.PayrollDate >= startDate
                && ep.PayrollDate <= endDate).AsQueryable();

            if(!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
            {
                employeePayrollByDate = FilterByEmployee(employeePayrollByDate, firstName, lastName);
            }

            return await employeePayrollByDate.ToListAsync();
        }

        private IQueryable<EmployeePayroll> FilterByEmployee(IQueryable<EmployeePayroll> employeePayrollByDate, string firstName, string lastName)
        {
            if(!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return employeePayrollByDate.Where(ep => ep.FirstName == firstName
                    && ep.LastName == lastName);
            }

            return employeePayrollByDate.Where(ep => ep.FirstName.ToLower().Contains(firstName)
                || ep.LastName.ToLower().Contains(lastName));
        }
    }
}
