using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IPayrollRepository
    {
        Task AddEmployeePayroll(List<EmployeePayroll> newPayrolls);
        Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id);
        Task<List<EmployeePayroll>> GetEmployeePayrollForCertainPeriodByEmployeeId(long id, DateTime startDate, DateTime endDate);
        Task UpdateEmployeePayroll(EmployeePayroll updatePayroll);
    }

    public class PayrollRepository : IPayrollRepository
    {
        /// <summary>
        /// Adding a pay period into payroll
        /// </summary>
        /// <param name="newPayrolls"></param>
        /// <returns></returns>
        public async Task AddEmployeePayroll(List<EmployeePayroll> newPayrolls)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayroll.AddRange(newPayrolls);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets entire payroll history of EE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var result = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id).ToListAsync();

            return result;
        }

        /// <summary>
        /// Gets payroll history between a certain period of EE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<EmployeePayroll>> GetEmployeePayrollForCertainPeriodByEmployeeId(long id, DateTime startDate, DateTime endDate)
        {
            using var context = new RofDatamartContext();

            var employeePayroll = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id 
                && ep.PayPeriodStartDate >= startDate 
                && ep.PayPeriodEndDate <= endDate).ToListAsync();

            return employeePayroll;
        }

        /// <summary>
        /// Updates payroll
        /// </summary>
        /// <param name="updatePayroll"></param>
        /// <returns></returns>
        public async Task UpdateEmployeePayroll(EmployeePayroll updatePayroll)
        {
            using var context = new RofDatamartContext();

            context.Update(updatePayroll);

            await context.SaveChangesAsync();
        }
    }
}
