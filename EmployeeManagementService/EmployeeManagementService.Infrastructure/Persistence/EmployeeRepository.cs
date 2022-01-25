using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public class EmployeeRepository
    {
        public async Task<List<Employee>> GetAllEmployees()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.Select(e => new Employee() { FirstName  = e.FirstName, LastName = e.LastName, Role = e.Role, Username = e.Username, Active = e.Active }).ToListAsync();
            }
        }

        public async Task CreateEmployee(string firstName, string lastName, string username, byte[] password, string role, bool active = true)
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
                    Active = active
                };

                context.Employees.Add(employee);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployee(long id, string firstName, string lastName, string role, bool active)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    throw new ArgumentException("No employee found.");
                }

                employee.FirstName = firstName;
                employee.LastName = lastName;
                employee.Role = role;
                employee.Active = active;

                await context.SaveChangesAsync();
            }
        }
    }
}
