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
        Task CreateEmployee(string firstName, string lastName, string username, string ssn, byte[] password, string role, string a1, string a2, string city, string state, string zip, bool? active = true);
        Task<Employee> GetAddressById(long id);
        Task<List<Employee>> GetAllEmployees(int page = 1, int offset = 10);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task<int> IncrementEmployeeFailedLoginAttempt(long id);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateAddress(string a1, string a2, string city, string state, string zip);
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

                return await context.Employees.Select(e => CreateGetAllEmployeeObj(e)).Skip(skip).Take(offset).ToListAsync();
            }
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.Select(e => CreateGetEmployeeByIdObj(e)).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.FirstOrDefaultAsync(e => e.Username == username);
            }
        }

        public async Task CreateEmployee(string firstName, string lastName, string username, string ssn, byte[] password, string role, string a1, string a2, string city, string state, string zip, bool? active = true)
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
                    Active = active,
                    AddressLine1 = a1,
                    AddressLine2 = a2,
                    City = city,
                    State = state,
                    ZipCode = zip
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

        public async Task<Employee> GetAddressById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.Select(e => CreateGetEmployeeAddressObj(e)).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task UpdateAddress(string a1, string a2, string city, string state, string zip)
        {
            using (var context = new RofSchedulerContext())
            {


                await context.SaveChangesAsync();
            }
        }

        private Employee CreateGetAllEmployeeObj(Employee e)
        {
            return new Employee()
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                Ssn = e.Ssn,
                Role = e.Role,
                Username = e.Username,
                Active = e.Active
            };
        }

        private Employee CreateGetEmployeeByIdObj(Employee e)
        {
            return new Employee()
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                Ssn = e.Ssn,
                Role = e.Role,
                Username = e.Username,
                Active = e.Active,
                IsLocked = e.IsLocked,
                FailedLoginAttempts = e.FailedLoginAttempts
            };
        }

        private Employee CreateGetEmployeeAddressObj(Employee e)
        {
            return new Employee()
            {
                AddressLine1 = e.AddressLine1,
                AddressLine2 = e.AddressLine2,
                City = e.City,
                State = e.State,
                ZipCode = e.ZipCode
            };
        }
    }
}
