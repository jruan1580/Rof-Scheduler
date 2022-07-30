using ClientManagementService.Domain.Models;
using CorePet = ClientManagementService.Domain.Models.Pet;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;

namespace ClientManagementService.Domain.Mappers.Database
{
    public class PetMapper
    {
        public static CorePet ToCorePet(DbPet dbPet)
        {
            var corePet = new CorePet();

            corePet.Id = dbPet.Id;
            corePet.OwnerId = dbPet.OwnerId;
            corePet.BreedId = dbPet.BreedId;
            corePet.Name = dbPet.Name;
            corePet.Weight = dbPet.Weight;
            corePet.Dob = dbPet.Dob;
            corePet.OtherInfo = dbPet.OtherInfo;

            corePet.Owner = new Client() { Id = dbPet.OwnerId, FirstName = dbPet.Owner.FirstName, LastName = dbPet.Owner.LastName };
            corePet.BreedInfo = new Breed() { Id = dbPet.BreedId, BreedName = dbPet.Breed.BreedName };

            return corePet;
        }

        public static DbPet FromCorePet(CorePet corePet)
        {
            var entity = new DbPet();

            entity.Id = corePet.Id;
            entity.OwnerId = corePet.Owner.Id;
            entity.BreedId = corePet.BreedInfo.Id;
            entity.Name = corePet.Name;
            entity.Weight = corePet.Weight;
            entity.Dob = corePet.Dob;
            entity.OtherInfo = corePet.OtherInfo;

            return entity;
        }
    }
}
