using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence.Entities;
using DbVaccine = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;

namespace ClientManagementService.Test
{
    public static class VaccineCreator
    {
        public static PetToVaccine GetDbPetToVaccine()
        {
            return new PetToVaccine()
            {
                Id = 1,
                PetId = 1,
                VaxId = 1,
                Inoculated = true,
                Vax = new DbVaccine()
                {
                    Id = 1,
                    PetTypeId = 1,
                    VaxName = "Bordetella"
                }
            };
        }

        public static VaccineStatus GetDomainVaccine()
        {
            return new VaccineStatus()
            {
                Id = 1,
                PetToVaccineId = 1,
                Inoculated = true,
                VaxName = "Bordetella"
            };
        }
    }
}
