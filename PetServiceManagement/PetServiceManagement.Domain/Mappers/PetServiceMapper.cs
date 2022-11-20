using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Domain.Mappers
{
    public static class PetServiceMapper
    {
        public static PetService ToDomainPetService(PetServices petServices)
        {
            if (petServices == null)
            {
                return null;
            }

            var petService = new PetService();

            petService.Id = petServices.Id;
            petService.Name = petServices.ServiceName;
            petService.Price = petServices.Price;
            petService.Description = petServices.Description;
            petService.EmployeeRate = petServices.EmployeeRate;
            petService.Duration = petServices.Duration;
            petService.TimeUnit = petServices.TimeUnit;

            return petService;
        }

        public static PetServices FromDomainPetService(PetService petService)
        {
            if (petService == null)
            {
                return null;
            }

            var petServices = new PetServices();

            petServices.Id = petService.Id;
            petServices.ServiceName = petService.Name;
            petServices.Price = petService.Price;
            petServices.EmployeeRate = petService.EmployeeRate;
            petServices.Description = petService.Description;
            petServices.Duration = petService.Duration;
            petServices.TimeUnit = petService.TimeUnit;

            return petServices;
        }
    }
}
