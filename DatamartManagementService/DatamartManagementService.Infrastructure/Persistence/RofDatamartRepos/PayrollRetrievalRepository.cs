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
        Task<List<EmployeePayroll>> GetEmployeePayrollForCertainPeriodByEmployeeId(long id, DateTime startDate, DateTime endDate);
    }

    public class PayrollRetrievalRepository : IPayrollRetrievalRepository
    {
        public async Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var employeePayroll = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id).ToListAsync();

            return employeePayroll;
        }

        public async Task<List<EmployeePayroll>> GetEmployeePayrollForCertainPeriodByEmployeeId(long id, DateTime startDate, DateTime endDate)
        {
            using var context = new RofDatamartContext();

            var employeePayrollByDate = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id
                && ep.PayrollDate >= startDate
                && ep.PayrollDate <= endDate).ToListAsync();

            return employeePayrollByDate;
        }
    }
}
