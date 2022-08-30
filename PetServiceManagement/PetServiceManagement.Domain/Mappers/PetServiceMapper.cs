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

            return petServices;
        }
    }
}
