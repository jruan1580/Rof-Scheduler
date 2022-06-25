using System.Collections.Generic;

namespace EmployeeManagementService.Domain.Models
{
    public class EmployeesWithTotalPage
    {
        public EmployeesWithTotalPage(List<Employee> employees, int totalPages)
        {
            Employees = employees;
            TotalPages = totalPages;
        }

        public List<Employee> Employees { get; set; }

        public int TotalPages { get; set; }       
    }
}
