using ClientManagementService.Domain.Models;
using ClientManagementService.DTO;
using System.Collections.Generic;
using CorePet = ClientManagementService.Domain.Models.Pet;
using CoreVaxStat = ClientManagementService.Domain.Models.VaccineStatus;

namespace ClientManagementService.Domain.Mappers.DTO
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
            dtoPet.PetTypeId = corePet.PetTypeId;
            dtoPet.PetTypeName = corePet.PetType.PetTypeName;

            dtoPet.Vaccines = new List<PetsVaccineDTO>();

            if (corePet.Vaccines != null)
            {
                foreach (var vax in corePet.Vaccines)
                {
                    dtoPet.Vaccines.Add(new PetsVaccineDTO()
                    {
                        Id = vax.Id,
                        PetsVaccineId = vax.PetToVaccineId,
                        VaccineName = vax.VaxName,
                        Inoculated = vax.Inoculated
                    });
                }
            }

            return dtoPet;
        }

        public static CorePet FromDTOPet(PetDTO dtoPet)
        {
            var corePet = new CorePet();

            corePet.Id = dtoPet.Id;
            corePet.OwnerId = dtoPet.OwnerId;
            corePet.PetTypeId = dtoPet.PetTypeId;
            corePet.BreedId = dtoPet.BreedId;
            corePet.Name = dtoPet.Name;
            corePet.Weight = dtoPet.Weight;
            corePet.Dob = dtoPet.Dob;
            corePet.OtherInfo = dtoPet.OtherInfo;
            corePet.Owner = new Client() { Id = dtoPet.OwnerId, FirstName = dtoPet.OwnerFirstName, LastName = dtoPet.OwnerLastName };
            corePet.BreedInfo = new Breed() { Id = dtoPet.BreedId, BreedName = dtoPet.BreedName };
            corePet.PetType = new PetType() { Id = dtoPet.PetTypeId, PetTypeName = dtoPet.PetTypeName };

            corePet.Vaccines = new List<VaccineStatus>();

            if (dtoPet.Vaccines != null)
            {
                foreach (var vax in dtoPet.Vaccines)
                {
                    corePet.Vaccines.Add(new VaccineStatus()
                    {
                        Id = vax.Id,
                        PetToVaccineId = vax.PetsVaccineId,
                        VaxName = vax.VaccineName,
                        Inoculated = vax.Inoculated
                    });
                }
            }      

            return corePet;
        }

        public static PetsVaccineDTO ToDTOPetsVaccine(CoreVaxStat coreVaxStat)
        {
            var dtoPetsVaccine = new PetsVaccineDTO();

            dtoPetsVaccine.Id = coreVaxStat.Id;
            dtoPetsVaccine.PetsVaccineId = coreVaxStat.PetToVaccineId;
            dtoPetsVaccine.VaccineName = coreVaxStat.VaxName;
            dtoPetsVaccine.Inoculated = coreVaxStat.Inoculated;

            return dtoPetsVaccine;
        }
    }
}
