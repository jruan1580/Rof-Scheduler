using CoreVax = ClientManagementService.Domain.Models.Vaccine;
using DbVax = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;

namespace ClientManagementService.Domain.Mappers.Database
{
    public class VaccineMapper
    {
        public static CoreVax ToCorePet(DbVax dbPet)
        {
            var coreVax = new CoreVax();

            coreVax.Id = dbPet.Id;
            coreVax.VaxName = dbPet.VaxName;

            return coreVax;
        }
    }
}
