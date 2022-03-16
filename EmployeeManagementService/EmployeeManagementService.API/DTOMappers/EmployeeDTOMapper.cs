using DTOEmployee = EmployeeManagementService.API.DTO.EmployeeDTO;
using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;

namespace EmployeeManagementService.API.DTOMappers
{
    public class EmployeeDTOMapper
    {
        public static DTOEmployee ToDTOEmployee(CoreEmployee coreEmp)
        {
            var dtoEmp = new DTOEmployee();

            dtoEmp.Id = coreEmp.Id;
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

            return dtoEmp;
        }

        public static CoreEmployee FromDTOEmployee(DTOEmployee dtoEmp)
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = dtoEmp.Id;
            coreEmp.FirstName = dtoEmp.FirstName;
            coreEmp.LastName = dtoEmp.LastName;
            coreEmp.Ssn = dtoEmp.Ssn;
            coreEmp.Username = dtoEmp.Username;
            coreEmp.Role = dtoEmp.Role;
            coreEmp.IsLocked = dtoEmp.IsLocked;
            coreEmp.FailedLoginAttempts = dtoEmp.FailedLoginAttempts;
            coreEmp.TempPasswordChanged = dtoEmp.TempPasswordChanged;
            coreEmp.Status = dtoEmp.Status;
            coreEmp.Active = dtoEmp.Active;

            return coreEmp;
        }
    }
}
