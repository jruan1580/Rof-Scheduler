using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofDatamartRepos
{
    public interface IPayrollDetailUpsertRepository
    {
        Task AddEmployeePayrollDetail(List<EmployeePayrollDetail> newPayrollDetails);
        Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail);
    }

    public class PayrollDetailUpsertRepository : IPayrollDetailUpsertRepository
    {
        public async Task AddEmployeePayrollDetail(List<EmployeePayrollDetail> newPayrollDetails)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayrollDetail.AddRange(newPayrollDetails);

            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeePayrollDetail(EmployeePayrollDetail updatePayrollDetail)
        {
            using var context = new RofDatamartContext();

            context.Update(updatePayrollDetail);

            await context.SaveChangesAsync();
        }
    }
}
