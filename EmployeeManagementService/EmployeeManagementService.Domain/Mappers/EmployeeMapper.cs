using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Mappers
{
    public class EmployeeMapper
    {
        public static CoreEmployee DbEmployeeToCoreEmployee(DbEmployee dbEmployee)
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = dbEmployee.Id;
            coreEmp.FirstName = dbEmployee.FirstName;
            coreEmp.LastName = dbEmployee.LastName;
            coreEmp.Ssn = dbEmployee.Ssn;
            coreEmp.Username = dbEmployee.Username;
            coreEmp.Password = dbEmployee.Password;
            coreEmp.Role = dbEmployee.Role;
            coreEmp.IsLocked = dbEmployee.IsLocked;
            coreEmp.FailedLoginAttempts = dbEmployee.FailedLoginAttempts;
            coreEmp.TempPasswordChanged = dbEmployee.TempPasswordChanged;
            coreEmp.Status = dbEmployee.Status;
            coreEmp.Active = dbEmployee.Active;

            return coreEmp;
        }
    }
}
