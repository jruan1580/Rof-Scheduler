using CorePet = ClientManagementService.Domain.Models.Pet;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using CorePetType = ClientManagementService.Domain.Models.PetType;
using DbPetType = ClientManagementService.Infrastructure.Persistence.Entities.PetType;
using CoreBreed = ClientManagementService.Domain.Models.Breed;
using DbBreed = ClientManagementService.Infrastructure.Persistence.Entities.Breed;

using ClientManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;

namespace ClientManagementService.Domain.Mappers.Database
{
    public static class PetMapper
    {
        public static CorePet ToCorePet(DbPet dbPet, List<PetToVaccine> petToVaccine)
        {
            var corePet = new CorePet();

            corePet.Id = dbPet.Id;
            corePet.OwnerId = dbPet.OwnerId;
            corePet.PetTypeId = dbPet.PetTypeId;
            corePet.BreedId = dbPet.BreedId;
            corePet.Name = dbPet.Name;
            corePet.Weight = dbPet.Weight;
            corePet.Dob = dbPet.Dob;
            corePet.OtherInfo = dbPet.OtherInfo;

            corePet.Owner = new Models.Client() { Id = dbPet.OwnerId, FirstName = dbPet.Owner.FirstName, LastName = dbPet.Owner.LastName };
            corePet.BreedInfo = new Models.Breed() { Id = dbPet.BreedId, BreedName = dbPet.Breed.BreedName, PetTypeId = dbPet.PetTypeId };
            corePet.PetType = new CorePetType() { Id = dbPet.PetTypeId, PetTypeName = dbPet.PetType.PetTypeName };

            if (petToVaccine != null && petToVaccine.Count > 0)
            {
                corePet.Vaccines = PetToVaccineMapper.ToVaccineStatus(petToVaccine);
            }

            return corePet;
        }

        public static DbPet FromCorePet(CorePet corePet)
        {
            var entity = new DbPet();

            entity.Id = corePet.Id;
            entity.OwnerId = corePet.OwnerId;
            entity.BreedId = corePet.BreedId;
            entity.PetTypeId = corePet.PetTypeId;
            entity.Name = corePet.Name;
            entity.Weight = corePet.Weight;
            entity.Dob = corePet.Dob;
            entity.OtherInfo = corePet.OtherInfo;

            return entity;
        }

        public static CorePetType ToCorePetType(DbPetType dbPetType)
        {
            var corePetType = new CorePetType();

            corePetType.Id = dbPetType.Id;
            corePetType.PetTypeName = dbPetType.PetTypeName;

            return corePetType;
        }

        public static CoreBreed ToCoreBreed(DbBreed dbBreed)
        {
            var coreBreed = new CoreBreed();

            coreBreed.Id = dbBreed.Id;
            coreBreed.BreedName = dbBreed.BreedName;

            return coreBreed;
        }
    }
}
