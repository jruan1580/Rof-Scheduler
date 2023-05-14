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
        Task AddEmployeePayroll(EmployeePayroll newPayroll);
        Task AddEmployeePayrollDetail(EmployeePayrollDetail newPayrollDetail);
        Task<List<EmployeePayroll>> GetEmployeePayrollByEmployeeId(long id);
        Task<List<EmployeePayrollDetail>> GetEmployeePayrollDetailByEmployeeId(long id);
        Task<List<EmployeePayroll>> GetEmployeePayrollForCertainPeriodByEmployeeId(long id, DateTime startDate, DateTime endDate);
        Task UpdateEmployeePayroll(EmployeePayroll updatePayroll);
        Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail);
    }

    public class PayrollRepository : IPayrollRepository
    {
        /// <summary>
        /// Adding a pay period into payroll
        /// </summary>
        /// <param name="newPayroll"></param>
        /// <returns></returns>
        public async Task AddEmployeePayroll(EmployeePayroll newPayroll)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayroll.Add(newPayroll);

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

            foreach (var payroll in employeePayroll)
            {
                if (payroll.PayPeriodStartDate >= startDate && payroll.PayPeriodEndDate <= endDate)
                {
                    result.Add(payroll);
                }
            }

            return result;
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

        /// <summary>
        /// Adds  detailed payroll info
        /// </summary>
        /// <param name="newPayrollDetail"></param>
        /// <returns></returns>
        public async Task AddEmployeePayrollDetail(EmployeePayrollDetail newPayrollDetail)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayrollDetail.Add(newPayrollDetail);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets list of detailed payroll info for employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<EmployeePayrollDetail>> GetEmployeePayrollDetailByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var result = await context.EmployeePayrollDetail.Where(pd => pd.EmployeeId == id).ToListAsync();

            return result;
        }

        /// <summary>
        /// Updates detailed payroll info
        /// </summary>
        /// <param name="updatePayrollDetail"></param>
        /// <returns></returns>
        public async Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail)
        {
            using var context = new RofDatamartContext();

            context.Update(updatePayrollDetail);

            await context.SaveChangesAsync();
        }
    }
}
