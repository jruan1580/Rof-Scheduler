using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.EntityFrameworkCore;
using RofShared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public interface IEmployeeRetrievalRepository
    {
        Task<bool> DoesEmployeeExistsBySsnOrUsernameOrEmail(string ssn, string username, string email, long id);
        Task<(List<Employee>, int)> GetAllEmployeesByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<Employee> GetEmployeeByFilter<T>(GetEmployeeFilterModel<T> filter);
        Task<List<Employee>> GetEmployeesForDropdown();
    }

    public class EmployeeRetrievalRepository : IEmployeeRetrievalRepository
    {
        public async Task<List<Employee>> GetEmployeesForDropdown()
        {
            using var context = new RofSchedulerContext();

            return await context.Employees.ToListAsync();
        }

        public async Task<(List<Employee>, int)> GetAllEmployeesByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using var context = new RofSchedulerContext();

            var skip = (page - 1) * offset;
            var employees = FilterByKeyword(context, keyword?.Trim()?.ToLower());

            var countByCriteria = await employees.CountAsync();

            var totalPages = DatabaseUtilities.GetTotalPages(countByCriteria, offset, page);

            var resut = await employees
                .OrderByDescending(e => e.Id)
                .Skip(skip)
                .Take(offset)
                .ToListAsync();

            return (resut, totalPages);
        }

        public async Task<Employee> GetEmployeeByFilter<T>(GetEmployeeFilterModel<T> filter)
        {
            using var context = new RofSchedulerContext();

            if (filter.FilterType == GetEmployeeFilterEnum.Id)
            {
                var val = Convert.ToInt64(filter.Value);

                return await context.Employees.FirstOrDefaultAsync(e => e.Id == val);
            }
            else if (filter.FilterType == GetEmployeeFilterEnum.Usermame)
            {
                var username = Convert.ToString(filter.Value).ToLower();

                return await context.Employees.FirstOrDefaultAsync(e => e.Username.ToLower().Equals(username));
            }
            else
            {
                throw new ArgumentException("Invalid Filter Type");
            }
        }

        public async Task<bool> DoesEmployeeExistsBySsnOrUsernameOrEmail(string ssn, string username, string email, long id)
        {
            using var context = new RofSchedulerContext();

            return await context.Employees.AnyAsync(e => e.Id != id
                && (e.Ssn.Equals(ssn)
                    || e.Username.ToLower().Equals(username.ToLower())
                    || e.EmailAddress.ToLower().Equals(email.ToLower())));
        }

        private IQueryable<Employee> FilterByKeyword(RofSchedulerContext context, string keyword)
        {
            var employees = context.Employees.AsQueryable();

            if (string.IsNullOrEmpty(keyword))
            {
                return employees;
            }

            return employees.Where(e => (e.FirstName.ToLower().Contains(keyword)) || 
                (e.LastName.ToLower().Contains(keyword)) || 
                (e.EmailAddress.ToLower().Contains(keyword)));
        }
    }
}
