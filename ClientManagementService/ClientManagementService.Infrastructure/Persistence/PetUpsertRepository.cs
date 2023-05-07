using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using RofShared.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IPetUpsertRepository
    {
        Task<long> AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task UpdatePet(Pet updatePet);
    }

    public class PetUpsertRepository : IPetUpsertRepository
    {
        public async Task<long> AddPet(Pet newPet)
        {
            using var context = new RofSchedulerContext();

            context.Pets.Add(newPet);

            await context.SaveChangesAsync();

            return newPet.Id;
        }

        public async Task UpdatePet(Pet updatePet)
        {
            using var context = new RofSchedulerContext();

            context.Pets.Update(updatePet);

            await context.SaveChangesAsync();
        }

        public async Task DeletePetById(long petId)
        {
            using var context = new RofSchedulerContext();

            var petToVax = await context.PetToVaccines.Where(v => v.PetId == petId).ToListAsync();

            var pet = await context.Pets.FirstOrDefaultAsync(p => p.Id == petId);

            if (pet == null)
            {
                throw new EntityNotFoundException("Pet");
            }

            context.RemoveRange(petToVax);
            context.Remove(pet);

            await context.SaveChangesAsync();
        }
    }
}
