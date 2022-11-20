using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;

namespace PetServiceManagement.API.DtoMapper
{
    public static class PetServiceDtoMapper
    {
        public static PetServiceDTO ToPetServiceDTO(PetService petServiceDomain)
        {
            if (petServiceDomain == null)
            {
                return null;
            }

            var dto = new PetServiceDTO();

            dto.Id = petServiceDomain.Id;
            dto.Name = petServiceDomain.Name;
            dto.Rate = petServiceDomain.Price;
            dto.Description = petServiceDomain.Description;
            dto.EmployeeRate = petServiceDomain.EmployeeRate;
            dto.Duration = petServiceDomain.Duration;
            dto.TimeUnit = petServiceDomain.TimeUnit;

            return dto;
        }

        public static PetService FromPetServiceDTO(PetServiceDTO petServiceDTO)
        {
            if (petServiceDTO == null)
            {
                return null;
            }

            var domain = new PetService();

            domain.Id = petServiceDTO.Id;
            domain.Name = petServiceDTO.Name;
            domain.Description = petServiceDTO.Description;
            domain.Price = petServiceDTO.Rate;
            domain.EmployeeRate = petServiceDTO.EmployeeRate;
            domain.Duration = petServiceDTO.Duration;
            domain.TimeUnit = petServiceDTO.TimeUnit;

            return domain;
        }
    }
}
