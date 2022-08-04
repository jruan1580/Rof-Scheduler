using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using CoreVax = ClientManagementService.Domain.Models.Vaccine;
using DbVax = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;

namespace ClientManagementService.Domain.Mappers.Database
{
    public static class PetToVaccineMapper
    {
        public static CoreVax ToCoreVax(DbVax dbPet)
        {
            var coreVax = new CoreVax();

            coreVax.Id = dbPet.Id;
            coreVax.VaxName = dbPet.VaxName;

            return coreVax;
        }

        public static List<VaccineStatus> ToVaccineStatus(List<PetToVaccine> petToVaccines)
        {
            var vaccineStatuses = new List<VaccineStatus>();

            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                return vaccineStatuses;
            }

            foreach (var petVaccine in petToVaccines)
            {
                var vaccineStatus = new VaccineStatus();

                vaccineStatus.Id = (short)petVaccine.VaxId;
                vaccineStatus.VaxName = petVaccine.Vax.VaxName;
                vaccineStatus.Inoculated = (bool)petVaccine.Inoculated;
                vaccineStatus.PetToVaccineId = petVaccine.Id;
            }

            return vaccineStatuses;
        }

        public static List<PetToVaccine> ToPetToVaccine(long petId, List<VaccineStatus> vaccineStatus)
        {
            var petToVaccines = new List<PetToVaccine>();

            if (vaccineStatus == null || vaccineStatus.Count == 0)
            {
                return petToVaccines;
            }

            foreach(var vaccine in vaccineStatus)
            {
                var petToVaccine = new PetToVaccine();

                petToVaccine.VaxId = vaccine.Id;
                petToVaccine.PetId = petId;
                petToVaccine.Inoculated = vaccine.Inoculated;
                petToVaccine.Id = vaccine.PetToVaccineId;
            }

            return petToVaccines;
        }
    }
}
