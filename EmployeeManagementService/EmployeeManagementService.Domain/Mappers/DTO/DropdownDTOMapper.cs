using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.DTO;
using System.Collections.Generic;

namespace EmployeeManagementService.Domain.Mappers.DTO
{
    public static class DropdownDTOMapper
    {
        public static List<EmployeeDTO> ToEmployeeDTO(List<Employee> employees)
        {
            var dtos = new List<EmployeeDTO>();

            foreach(var employee in employees)
            {
                dtos.Add(new EmployeeDTO()
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    FullName = employee.FullName
                });
            }

            return dtos;
        }
    }
}
