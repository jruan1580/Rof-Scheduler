using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IVaccinesRepository
    {
        Task<List<Vaccine>> GetVaccinesByPetType(int petTypeId);
    }

    public class VaccinesRepository
    {
        /// <summary>
        /// Gets all vaccines for related pet type (eg. dog, cat..etc)
        /// </summary>
        /// <param name="petTypeId"></param>
        /// <returns></returns>
        public async Task<List<Vaccine>> GetVaccinesByPetType(int petTypeId)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Vaccines.Where(v => v.PetTypeId == petTypeId).ToListAsync();
            }
        }
    }
}
