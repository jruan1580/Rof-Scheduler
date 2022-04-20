using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public interface IEmployeeRepository
    {
        Task CreateEmployee(Employee newEmployee);
        Task<List<Employee>> GetAllEmployees(int page = 1, int offset = 10);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task<int> IncrementEmployeeFailedLoginAttempt(long id);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(Employee employeeToUpdate);
        Task UpdateEmployeeIsLockedStatus(long id, bool isLocked);
        Task UpdateEmployeeLoginStatus(long id, bool status);
        Task UpdatePassword(long id, byte[] newPassword);
        Task DeleteEmployeeById(long id);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        public async Task<List<Employee>> GetAllEmployees(int page = 1, int offset = 10)
        {
            using (var context = new RofSchedulerContext())
            {
                var count = await context.Employees.CountAsync();

                var totalPages = Math.Ceiling(count / (double)offset);

                if (page > totalPages)
                {
                    throw new ArgumentException("No more employees.");
                }

                var skip = (page - 1) * offset;

                return await context.Employees
                    .Select(e => new Employee()
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Ssn = e.Ssn,
                        Role = e.Role,
                        Username = e.Username,
                        Active = e.Active,
                        AddressLine1 = e.AddressLine1,
                        AddressLine2 = e.AddressLine2,
                        City = e.City,
                        State = e.State,
                        ZipCode = e.ZipCode,
                        CountryId = e.CountryId
                    })
                    .Skip(skip)
                    .Take(offset)
                    .ToListAsync();
            }
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees
                    .Where(e => e.Id == id)
                    .Select(e => new Employee()
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Ssn = e.Ssn,
                        Role = e.Role,
                        Username = e.Username,
                        Active = e.Active,
                        IsLocked = e.IsLocked,
                        FailedLoginAttempts = e.FailedLoginAttempts,
                        AddressLine1 = e.AddressLine1,
                        AddressLine2 = e.AddressLine2,
                        City = e.City,
                        State = e.State,
                        ZipCode = e.ZipCode,
                        CountryId = e.CountryId,
                        Status = e.Status
                    })
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.Include(c => c.Country).FirstOrDefaultAsync(e => e.Username == username);
            }
        }

        public async Task CreateEmployee(Employee newEmployee)
        {
            using (var context = new RofSchedulerContext())
            {                
                context.Employees.Add(newEmployee);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeInformation(Employee employeeToUpdate)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Employees.Update(employeeToUpdate);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeLoginStatus(long id, bool status)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.Status = status;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeActiveStatus(long id, bool active)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.Active = active;

                await context.SaveChangesAsync();
            }
        }

        public async Task<int> IncrementEmployeeFailedLoginAttempt(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.FailedLoginAttempts += 1;

                await context.SaveChangesAsync();

                return employee.FailedLoginAttempts;
            }
        }

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.FailedLoginAttempts = 0;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeIsLockedStatus(long id, bool isLocked)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.IsLocked = isLocked;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePassword(long id, byte[] newPassword)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.Password = newPassword;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployeeById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("Employee does not exist.");
                }

                context.Remove(employee);

                await context.SaveChangesAsync();
            }
        }
    }
}
