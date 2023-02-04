using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using EmployeeManagementService.DTO;

namespace EmployeeManagementService.Domain.Mappers.DTO
{
    public static class EmployeeDTOMapper
    {
        public static EmployeeDTO ToDTOEmployee(CoreEmployee coreEmp)
        {
            var dtoEmp = new EmployeeDTO();

            dtoEmp.Id = coreEmp.Id;
            dtoEmp.CountryId = coreEmp.CountryId;
            dtoEmp.FirstName = coreEmp.FirstName;
            dtoEmp.LastName = coreEmp.LastName;
            dtoEmp.Ssn = coreEmp.Ssn;
            dtoEmp.Username = coreEmp.Username;
            dtoEmp.Role = coreEmp.Role;
            dtoEmp.IsLocked = coreEmp.IsLocked;
            dtoEmp.FailedLoginAttempts = coreEmp.FailedLoginAttempts;
            dtoEmp.TempPasswordChanged = coreEmp.TempPasswordChanged;
            dtoEmp.Status = coreEmp.Status;
            dtoEmp.Active = coreEmp.Active;
            dtoEmp.FullName = coreEmp.FullName;
            dtoEmp.EmailAddress = coreEmp.Email;
            dtoEmp.PhoneNumber = coreEmp.PhoneNumber;

            dtoEmp.Address = new AddressDTO();
            dtoEmp.Address.AddressLine1 = coreEmp.Address?.AddressLine1;
            dtoEmp.Address.AddressLine2 = coreEmp.Address?.AddressLine2;
            dtoEmp.Address.City = coreEmp.Address?.City;
            dtoEmp.Address.State = coreEmp.Address?.State;
            dtoEmp.Address.ZipCode = coreEmp.Address?.ZipCode;


            return dtoEmp;
        }

        public static CoreEmployee FromDTOEmployee(EmployeeDTO dtoEmp)
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = dtoEmp.Id;
            coreEmp.CountryId = dtoEmp.CountryId;
            coreEmp.FirstName = dtoEmp.FirstName;
            coreEmp.LastName = dtoEmp.LastName;
            coreEmp.Email = dtoEmp.EmailAddress;
            coreEmp.PhoneNumber = dtoEmp.PhoneNumber;

            coreEmp.SetFullName();

            //coreEmp.SetSSN(dtoEmp.Ssn);
            coreEmp.Ssn = dtoEmp.Ssn;
            
            coreEmp.Username = dtoEmp.Username;
            coreEmp.Role = dtoEmp.Role;
            coreEmp.IsLocked = dtoEmp.IsLocked;
            coreEmp.FailedLoginAttempts = dtoEmp.FailedLoginAttempts;
            coreEmp.TempPasswordChanged = dtoEmp.TempPasswordChanged;
            coreEmp.Status = dtoEmp.Status;
            coreEmp.Active = dtoEmp.Active;

            coreEmp.SetAddress(dtoEmp.Address?.AddressLine1, dtoEmp.Address?.AddressLine2, dtoEmp.Address?.City, dtoEmp.Address?.State, dtoEmp.Address?.ZipCode);          

            return coreEmp;
        }
    }
}
