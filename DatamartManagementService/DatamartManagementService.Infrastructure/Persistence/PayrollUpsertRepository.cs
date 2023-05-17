using DatamartManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IPayrollUpsertRepository
    {
        Task AddEmployeePayroll(List<EmployeePayroll> newPayrolls);
        Task UpdateEmployeePayroll(EmployeePayroll updatePayroll);
    }

    public class PayrollUpsertRepository : IPayrollUpsertRepository
    {
        public async Task AddEmployeePayroll(List<EmployeePayroll> newPayrolls)
        {
            using var context = new RofDatamartContext();

            context.EmployeePayroll.AddRange(newPayrolls);

            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeePayroll(EmployeePayroll updatePayroll)
        {
            using var context = new RofDatamartContext();

            context.Update(updatePayroll);

            await context.SaveChangesAsync();
        }
    }
}
