using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task CreateEmployee( string firstName, string lastName, string username, byte[] password, string role, bool active = true)
        {
            using (var context = new RofSchedulerContext())
            {
                var employee = new Employee()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Password = password,
                    Role = role  
                };

                context.Employees.Add(employee);

                await context.SaveChangesAsync();
            }
        }
    }
}
