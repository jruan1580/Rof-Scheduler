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
        public async Task AddEmployeePayrollDetail(List<EmployeePayrollDetail> newPayrollDetails)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayrollDetail.AddRange(newPayrollDetails);

            await context.SaveChangesAsync();
        }

        public async Task<List<EmployeePayrollDetail>> GetEmployeePayrollDetailByEmployeeId(long id)
        {
            using var context = new RofDatamartContext();

            var result = await context.EmployeePayrollDetail.Where(pd => pd.EmployeeId == id).ToListAsync();

            return result;
        }

        public async Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail)
        {
            using var context = new RofDatamartContext();

            context.Update(updatePayrollDetail);

            await context.SaveChangesAsync();
        }
    }
}
