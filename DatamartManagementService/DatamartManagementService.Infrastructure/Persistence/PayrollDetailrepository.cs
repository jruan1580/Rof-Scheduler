using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IPayrollDetailrepository
    {
        Task AddEmployeePayrollDetail(List<EmployeePayrollDetail> newPayrollDetails);
        Task<List<EmployeePayrollDetail>> GetEmployeePayrollDetailByEmployeeId(long id);
        Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail);
    }

    public class PayrollDetailrepository : IPayrollDetailrepository
    {
        /// <summary>
        /// Inserting list of payroll details
        /// </summary>
        /// <param name="newPayrollDetails"></param>
        /// <returns></returns>
        public async Task AddEmployeePayrollDetail(List<EmployeePayrollDetail> newPayrollDetails)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayrollDetail.AddRange(newPayrollDetails);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get payroll details of a certain employee
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
        /// Updating payroll detail of employee
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
