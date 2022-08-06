using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IPetToVaccinesRepository
    {
        Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId);
        Task<List<PetToVaccine>> GetPetToVaccineByPetId(long petId);

        //takes a list because a pet usually have more than one vax. so adding/updating a list of vax is more common
        Task AddPetToVaccines(List<PetToVaccine> petToVaccines);
        Task UpdatePetToVaccines(List<PetToVaccine> petToVaccines);
    }

    public class PetToVaccinesRepository : IPetToVaccinesRepository
    {
        /// <summary>
        /// Gets all vaccines for related pet type (eg. dog, cat..etc)
        /// </summary>
        /// <param name="petTypeId"></param>
        /// <returns></returns>
        public async Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Vaccines.Where(v => v.PetTypeId == petTypeId).ToListAsync();
            }
        }

        /// <summary>
        /// Gets all vax for a pet by pet id.
        /// If pet cannot be found, return a null list
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        public async Task<List<PetToVaccine>> GetPetToVaccineByPetId(long petId)
        {
            using(var context = new RofSchedulerContext())
            {
                var petExists = await context.Pets.AnyAsync(p => p.Id == petId);

                //returns null if unable to find pet
                if (!petExists)
                {
                    return null;
                }

                var petToVaccines = await context.PetToVaccines.Where(p => p.PetId == petId).ToListAsync();
                
                var vaxIds = petToVaccines.Select(v => v.VaxId).Distinct().ToList();
                var vaccines = await context.Vaccines.Where(v => vaxIds.Any(id => id == v.Id)).ToListAsync();

                foreach(var petToVax in petToVaccines)
                {
                    petToVax.Vax = vaccines.First(v => v.Id == petToVax.VaxId);
                }

                return petToVaccines;
            }
        }

        /// <summary>
        /// Adds a list of pet to vaccines records.
        /// One pet can have many vax
        /// </summary>
        /// <param name="petToVaccines"></param>
        /// <returns></returns>
        public async Task AddPetToVaccines(List<PetToVaccine> petToVaccines)
        {
            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                return;
            }

            using (var context = new RofSchedulerContext())
            {
                context.PetToVaccines.AddRange(petToVaccines);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Takes a list of pet to vaccines and updates them in DB
        /// </summary>
        /// <param name="petToVaccines"></param>
        /// <returns></returns>
        public async Task UpdatePetToVaccines(List<PetToVaccine> petToVaccines)
        {
            if(petToVaccines == null || petToVaccines.Count == 0)
            {
                return;
            }

            using (var context = new RofSchedulerContext())
            {
                context.PetToVaccines.UpdateRange(petToVaccines);

                await context.SaveChangesAsync();
            }
        }
    }
}
