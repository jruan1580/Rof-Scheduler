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
        Task CreateEmployee(string firstName, string lastName, string username, string ssn, byte[] password, string role, bool active = true);
        Task<List<Employee>> GetAllEmployees(int page = 1, int offset = 10);
        Task<Employee> GetEmployeeById(long id);
        Task IncrementEmployeeFailedLoginAttempt(long id);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(long id, string username, string firstName, string lastName, string role, string ssn);
        Task UpdateEmployeeIsLockedStatus(long id, bool isLocked);
        Task UpdateEmployeeLoginStatus(long id, bool status);
        Task UpdatePassword(long id, byte[] newPassword);
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

                return await context.Employees.Select(e => new Employee() { FirstName = e.FirstName, LastName = e.LastName, Ssn = e.Ssn, Role = e.Role, Username = e.Username, Active = e.Active })
                                        .Skip(skip)
                                        .Take(offset)
                                        .ToListAsync();
            }
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.Select(e => new Employee() { FirstName = e.FirstName, LastName = e.LastName, Ssn = e.Ssn, Role = e.Role, Username = e.Username, Active = e.Active, IsLocked = e.IsLocked })
                                        .FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task CreateEmployee(string firstName, string lastName, string username, string ssn, byte[] password, string role, bool active = true)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = new Employee()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Password = password,
                    Role = role,
                    Ssn = ssn,
                    Active = active
                };

                context.Employees.Add(employee);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeInformation(long id, string username, string firstName, string lastName, string role, string ssn)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.Username = username;
                employee.FirstName = firstName;
                employee.LastName = lastName;
                employee.Role = role;
                employee.Ssn = ssn;

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

        public async Task IncrementEmployeeFailedLoginAttempt(long id)
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
    }
}
