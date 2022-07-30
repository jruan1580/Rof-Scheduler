using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Mappers.Database
{
    public static class EmployeeMapper
    {
        public static CoreEmployee ToCoreEmployee(DbEmployee dbEmployee)
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = dbEmployee.Id;
            coreEmp.CountryId = dbEmployee.CountryId;
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
            coreEmp.Email = dbEmployee.EmailAddress;
            coreEmp.PhoneNumber = dbEmployee.PhoneNumber;

            coreEmp.SetAddress(dbEmployee.AddressLine1, dbEmployee.AddressLine2, dbEmployee.City, dbEmployee.State, dbEmployee.ZipCode);

            coreEmp.Ssn = dbEmployee.Ssn;

            coreEmp.SetFullName();

            return coreEmp;
        }

        public static DbEmployee FromCoreEmployee(CoreEmployee coreEmployee)
        {
            var entity = new DbEmployee();

            entity.Id = coreEmployee.Id;
            entity.CountryId = coreEmployee.CountryId;
            entity.FirstName = coreEmployee.FirstName;
            entity.LastName = coreEmployee.LastName;
            entity.Username = coreEmployee.Username;
            entity.EmailAddress = coreEmployee.Email;
            entity.PhoneNumber = coreEmployee.PhoneNumber;
            entity.Password = coreEmployee.Password;
            entity.Role = coreEmployee.Role;
            entity.IsLocked = coreEmployee.IsLocked;
            entity.FailedLoginAttempts = coreEmployee.FailedLoginAttempts;
            entity.TempPasswordChanged = coreEmployee.TempPasswordChanged;
            entity.Status = coreEmployee.Status;
            entity.Active = coreEmployee.Active;            

            entity.AddressLine1 = coreEmployee.Address?.AddressLine1;
            entity.AddressLine2 = coreEmployee.Address?.AddressLine2;
            entity.City = coreEmployee.Address?.City;
            entity.State = coreEmployee.Address?.State;
            entity.ZipCode = coreEmployee.Address?.ZipCode;

            entity.Ssn = coreEmployee.Ssn;

            return entity;
        }
    }
}
