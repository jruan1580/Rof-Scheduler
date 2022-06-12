using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
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
        Task<(List<Employee>, int)> GetAllEmployeesByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<Employee> GetEmployeeByFilter<T>(GetEmployeeFilterModel<T> filter);
        Task<short> IncrementEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployee(Employee employeeToUpdate);
        Task DeleteEmployeeById(long id);
        Task<bool> DoesEmployeeExistsBySsnOrUsername(string ssn, string username, long id);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        public async Task<(List<Employee>, int)> GetAllEmployeesByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using (var context = new RofSchedulerContext())
            {              
                var skip = (page - 1) * offset;
                IQueryable<Employee> employees = context.Employees;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    employees = context.Employees
                        .Where(e => (e.FirstName.ToLower().Contains(keyword))
                            || (e.LastName.ToLower().Contains(keyword))
                            || (e.EmailAddress.ToLower().Contains(keyword)));
                }
                     
                var countByCriteria = await employees.CountAsync();
                var fullPages = countByCriteria / offset; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
                var remaining = countByCriteria % offset; //remaining will be 3 which will be an extra page
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

                if (page > totalPages)
                {
                    throw new ArgumentException("No more employees.");
                }

                var resut = await employees
                    .Select(e => new Employee()
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Password = e.Password,
                        Ssn = e.Ssn,
                        Role = e.Role,
                        Username = e.Username,
                        EmailAddress = e.EmailAddress,
                        PhoneNumber = e.PhoneNumber,
                        Active = e.Active,
                        AddressLine1 = e.AddressLine1,
                        AddressLine2 = e.AddressLine2,
                        City = e.City,
                        State = e.State,
                        ZipCode = e.ZipCode,
                        CountryId = e.CountryId
                    })
                    .OrderByDescending(e => e.Id)
                    .Skip(skip)
                    .Take(offset)
                    .ToListAsync();

                return (resut, totalPages);
            }
        }

        public async Task<Employee> GetEmployeeByFilter<T>(GetEmployeeFilterModel<T> filter)
        {
            using (var context = new RofSchedulerContext())
            {
                if (filter.FilterType == GetEmployeeFilterEnum.Id)
                {
                    return await context.Employees.FirstOrDefaultAsync(e => e.Id == Convert.ToInt64(filter.Value));
                }
                else if (filter.FilterType == GetEmployeeFilterEnum.Usermame)
                {
                    return await context.Employees.FirstOrDefaultAsync(e => e.Username.ToLower().Equals(Convert.ToString(filter.Value).ToLower()));
                }
                else
                {
                    throw new ArgumentException("Invalid Filter Type");
                }
            }
        }       

        public async Task CreateEmployee(Employee newEmployee)
        {
            using (var context = new RofSchedulerContext())
            {
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
        }

        public async Task UpdateEmployee(Employee employeeToUpdate)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Employees.Update(employeeToUpdate);

                await context.SaveChangesAsync();
            }
        }

        public async Task<short> IncrementEmployeeFailedLoginAttempt(long id)
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

        public async Task<bool> DoesEmployeeExistsBySsnOrUsername(string ssn, string username, long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Employees.AnyAsync(e => e.Id != id && (e.Ssn.Equals(ssn) 
                    || e.Username.ToLower().Equals(username.ToLower())));             
            }
        }
    }
}
