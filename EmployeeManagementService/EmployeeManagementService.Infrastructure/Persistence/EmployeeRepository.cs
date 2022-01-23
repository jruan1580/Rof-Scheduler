using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                return await context.Employees.ToListAsync();
            }
        }
    }
}
