using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
