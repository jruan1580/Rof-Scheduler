using ClientManagementService.Infrastructure.Persistence.Entities;

namespace ClientManagementService.Test
{
    public static class PetToVaccineCreator
    {
        public static PetToVaccine GetDbPetToVaccine()
        {
            return new PetToVaccine()
            {
                Id = 1,
                VaxId = 1,
                Inoculated = true,
                Vax = new Vaccine()
                {
                    Id = 1,
                    PetTypeId = 1,
                    VaxName = "Bordetella"
                }
            };
        }
    }
}
