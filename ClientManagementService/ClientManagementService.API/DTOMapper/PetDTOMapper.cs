using ClientManagementService.API.DTO;
using ClientManagementService.Domain.Models;
using CorePet = ClientManagementService.Domain.Models.Pet;

namespace ClientManagementService.API.DTOMapper
{
    public static class PetDTOMapper
    {
        public static PetDTO ToDTOPet(CorePet corePet)
        {
            var dtoPet = new PetDTO();

            dtoPet.Id = corePet.Id;
            dtoPet.OwnerId = corePet.OwnerId;
            dtoPet.BreedId = corePet.BreedId;
            dtoPet.Name = corePet.Name;
            dtoPet.Dob = corePet.Dob;
            dtoPet.Weight = corePet.Weight;
            dtoPet.OtherInfo = corePet.OtherInfo;
            dtoPet.OwnerFirstName = corePet.Owner.FirstName;
            dtoPet.OwnerLastName = corePet.Owner.LastName;
            dtoPet.BreedName = corePet.BreedInfo.BreedName;

            return dtoPet;
        }

        public static CorePet FromDTOPet(PetDTO dtoPet)
        {
            var corePet = new CorePet();

            corePet.Id = dtoPet.Id;
            corePet.OwnerId = dtoPet.OwnerId;
            corePet.BreedId = dtoPet.BreedId;
            corePet.Name = dtoPet.Name;
            corePet.Weight = dtoPet.Weight;
            corePet.Dob = dtoPet.Dob;
            corePet.OtherInfo = dtoPet.OtherInfo;
            corePet.Owner = new Client() { Id = dtoPet.OwnerId, FirstName = dtoPet.OwnerFirstName, LastName = dtoPet.OwnerLastName };
            corePet.BreedInfo = new Breed() { Id = dtoPet.BreedId, BreedName = dtoPet.BreedName };

            return corePet;
        }
    }
}
