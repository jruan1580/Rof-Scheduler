using ClientManagementService.DTO;
using System.Collections.Generic;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using DomainPet = ClientManagementService.Domain.Models.Pet;

namespace ClientManagementService.Test
{
    public static class PetCreator
    {
        public static DbPet GetDbPet()
        {
            return new DbPet()
            {
                Id = 1,
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Name = "Layla",
                Weight = 70,
                Dob = "3/14/2019",
                Owner = new Infrastructure.Persistence.Entities.Client()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe"
                },
                Breed = new Infrastructure.Persistence.Entities.Breed()
                {
                    Id = 1,
                    BreedName = "Golden Retriever"
                },
                PetType = new Infrastructure.Persistence.Entities.PetType()
                {
                    Id = 1,
                    PetTypeName = "Dog"
                }
            };
        }

        public static DomainPet GetDomainPet()
        {
            return new DomainPet()
            {
                Id = 1,
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Name = "Layla",
                Weight = 70,
                Dob = "3/14/2019",
                Owner = new Domain.Models.Client()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe"
                },
                BreedInfo = new Domain.Models.Breed()
                {
                    Id = 1,
                    BreedName = "Golden Retriever"
                },
                PetType = new Domain.Models.PetType()
                {
                    Id = 1,
                    PetTypeName = "Dog"
                }
            };
        }

        public static PetDTO GetPetDTO()
        {
            return new PetDTO()
            {
                Id = 1,
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Name = "Layla",
                Weight = 70,
                Dob = "3/14/2019",
                OwnerFirstName = "John",
                OwnerLastName = "Doe",
                BreedName = "Golden Retriever",
                Vaccines = new List<PetsVaccineDTO>()
                {
                    new PetsVaccineDTO()
                    {
                        Id = 1,
                        PetsVaccineId = 1,
                        VaccineName = "Bordetella",
                        Inoculated = true
                    }
                }
            };
        }
    }
}
