using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class PayrollRepository
    {
        /// <summary>
        /// Adding a pay period into payroll
        /// </summary>
        /// <param name="payroll"></param>
        /// <returns></returns>
        public async Task AddEmployeePayroll(EmployeePayroll payroll)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayroll.Add(payroll);

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

            var employeePayroll = await context.EmployeePayroll.Where(ep => ep.EmployeeId == id).ToListAsync();

            var result = new List<EmployeePayroll>();

            foreach(var payroll in employeePayroll)
            {
                if(payroll.PayPeriodStartDate >= startDate && payroll.PayPeriodEndDate <= endDate)
                {
                    result.Add(payroll);
                }
            }

            return result;
        }
    }
}
