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
            corePet.BordetellaVax = dbPet.BordetellaVax;
            corePet.Dhppvax = dbPet.Dhppvax;
            corePet.RabieVax = dbPet.RabieVax;
            corePet.OtherInfo = dbPet.OtherInfo;
            corePet.Picture = dbPet.Picture;

            return corePet;
        }

        public static DbPet FromCorePet(CorePet corePet)
        {
            var entity = new DbPet();

            entity.Id = corePet.Id;
            entity.OwnerId = corePet.OwnerId;
            entity.BreedId = corePet.BreedId;
            entity.Name = corePet.Name;
            entity.Weight = corePet.Weight;
            entity.Dob = corePet.Dob;
            entity.BordetellaVax = corePet.BordetellaVax;
            entity.Dhppvax = corePet.Dhppvax;
            entity.RabieVax = corePet.RabieVax;
            entity.OtherInfo = corePet.OtherInfo;
            entity.Picture = corePet.Picture;

            return entity;
        }
    }
}
