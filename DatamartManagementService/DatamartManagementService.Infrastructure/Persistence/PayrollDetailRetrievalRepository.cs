using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IPayrollDetailRetrievalRepository
    {
        Task<List<EmployeePayrollDetail>> GetEmployeePayrollDetailByEmployeeId(long id);
    }

    public class PayrollDetailRetrievalRepository : IPayrollDetailRetrievalRepository
    {
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
