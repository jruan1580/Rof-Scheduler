using ClientManagementService.Domain.Models;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using DbPetType = ClientManagementService.Infrastructure.Persistence.Entities.PetType;
using DbVaccine = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;
using DbBreed = ClientManagementService.Infrastructure.Persistence.Entities.Breed;

namespace ClientManagementService.Test
{
    public static class DropdownCreator
    {
        public static Client GetDomainClientForDropdown()
        {
            return new Client()
            {
                Id = 1,
                FullName = "Test User"
            };
        }

        public static Pet GetDomainPetForDropdown()
        {
            return new Pet()
            {
                Id = 1,
                Name = "Layla"
            };
        }

        public static PetType GetDomainPetTypeForDropdown()
        {
            return new PetType()
            {
                Id = 1,
                PetTypeName = "Dog"
            };
        }

        public static Vaccine GetDomainVaccineForDropdown()
        {
            return new Vaccine()
            {
                Id = 1,
                VaxName = "Bordetella"
            };
        }

        public static Breed GetDomainBreedForDropdown()
        {
            return new Breed()
            {
                Id = 1,
                BreedName = "Golden Retriever"
            };
        }

        public static DbClient GetDbClientForDropdown()
        {
            return new DbClient()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User"
            };
        }

        public static DbPet GetDbPetForDropdown()
        {
            return new DbPet()
            {
                Id = 1,
                Name = "Layla"
            };
        }

        public static DbPetType GetDbPetTypeForDropdown()
        {
            return new DbPetType()
            {
                Id = 1,
                PetTypeName = "Dog"
            };
        }

        public static DbVaccine GetDbVaccineForDropdown()
        {
            return new DbVaccine()
            {
                Id = 1,
                VaxName = "Bordetella"
            };
        }

        public static DbBreed GetDbBreedForDropdown()
        {
            return new DbBreed()
            {
                Id = 1,
                BreedName = "Golden Retriever"
            };
        }
    }
}
