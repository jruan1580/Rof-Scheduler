using AutoMapper;
using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;


namespace EmployeeManagementService.Domain.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DbEmployee, CoreEmployee>();
        }
    }
}
