using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Tests
{
    public static class PetServiceFactory
    {
        public static PetService GetPetServiceDomain()
        {
            return new PetService()
            {
                Id = 1,
                Name = "Dog Walking",
                Description = "Walking dog",
                Price = 20m,
                EmployeeRate = 10m,
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };
        }

        public static PetServices GetPetServicesDbEntity()
        {
            return new PetServices()
            {
                Id = 1,
                Price = 20m,
                EmployeeRate = 10m,
                ServiceName = "Dog Walking",
                Description = "Walking dog",
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };
        }

        public static PetServiceDTO GetPetServiceDTO()
        {
            return new PetServiceDTO()
            {
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m,
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };
        }
    }
}
