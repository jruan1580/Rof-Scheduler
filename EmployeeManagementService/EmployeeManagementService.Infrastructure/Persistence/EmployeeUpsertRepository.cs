using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using RofShared.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public interface IEmployeeUpsertRepository
    {
        Task CreateEmployee(Employee newEmployee);
        Task DeleteEmployeeById(long id);
        Task<short> IncrementEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployee(Employee employeeToUpdate);
    }

    public class EmployeeUpsertRepository : IEmployeeUpsertRepository
    {
        public async Task CreateEmployee(Employee newEmployee)
        {
            using var context = new RofSchedulerContext();

            //default employee's country to USA for now
            var usa = context.Countries.FirstOrDefault(c => c.Name.Equals("United States of America"));
            if (usa == null)
            {
                throw new Exception("Unable to find country United States of America");
            }

            newEmployee.CountryId = usa.Id;
            context.Employees.Add(newEmployee);

            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployee(Employee employeeToUpdate)
        {
            using var context = new RofSchedulerContext();

            context.Employees.Update(employeeToUpdate);

            await context.SaveChangesAsync();
        }

        public async Task<short> IncrementEmployeeFailedLoginAttempt(long id)
        {
            using var context = new RofSchedulerContext();

            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            employee.FailedLoginAttempts += 1;

            await context.SaveChangesAsync();

            return employee.FailedLoginAttempts;
        }

        public async Task DeleteEmployeeById(long id)
        {
            using var context = new RofSchedulerContext();

            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            context.Remove(employee);

            await context.SaveChangesAsync();
        }
    }
}
