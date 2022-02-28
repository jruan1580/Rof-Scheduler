using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Mappers.Database
{
    public class EmployeeMapper
    {
        public static CoreEmployee ToCoreEmployee(DbEmployee dbEmployee)
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = dbEmployee.Id;
            coreEmp.FirstName = dbEmployee.FirstName;
            coreEmp.LastName = dbEmployee.LastName;           
            coreEmp.Username = dbEmployee.Username;
            coreEmp.Password = dbEmployee.Password;
            coreEmp.Role = dbEmployee.Role;
            coreEmp.IsLocked = dbEmployee.IsLocked;
            coreEmp.FailedLoginAttempts = dbEmployee.FailedLoginAttempts;
            coreEmp.TempPasswordChanged = dbEmployee.TempPasswordChanged;
            coreEmp.Status = dbEmployee.Status;
            coreEmp.Active = dbEmployee.Active;

            coreEmp.SetSSN(dbEmployee.Ssn);

            coreEmp.SetFullName();

            return coreEmp;
        }

        public static DbEmployee FromCoreEmployee(CoreEmployee coreEmployee)
        {
            var entity = new DbEmployee();

            entity.Id = coreEmployee.Id;
            entity.FirstName = coreEmployee.FirstName;
            entity.LastName = coreEmployee.LastName;
            entity.Username = coreEmployee.Username;
            entity.Password = coreEmployee.Password;
            entity.Role = coreEmployee.Role;
            entity.IsLocked = coreEmployee.IsLocked;
            entity.FailedLoginAttempts = coreEmployee.FailedLoginAttempts;
            entity.TempPasswordChanged = coreEmployee.TempPasswordChanged;
            entity.Status = coreEmployee.Status;
            entity.Active = coreEmployee.Active;

            entity.Ssn = coreEmployee.GetNotSanitizedSSN();

            return entity;
        }
    }
}
