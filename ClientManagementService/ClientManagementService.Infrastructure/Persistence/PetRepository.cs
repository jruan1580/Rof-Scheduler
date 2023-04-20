using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IPetRepository
    {
        Task<long> AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task UpdatePet(Pet updatePet);
    }

    public class PetRepository : IPetRepository
    {
        public async Task<long> AddPet(Pet newPet)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Pets.Add(newPet);

                await context.SaveChangesAsync();

                return newPet.Id;
            }
        }

        public async Task UpdatePet(Pet updatePet)
        {
            using (var context = new RofSchedulerContext())
            {
                var origPet = await context.Pets.FirstOrDefaultAsync(p => p.Id == updatePet.Id);

                origPet.Name = updatePet.Name;
                origPet.Weight = updatePet.Weight;
                origPet.Dob = updatePet.Dob;
           
                origPet.BreedId = updatePet.BreedId;
                origPet.OwnerId = updatePet.OwnerId;
                origPet.OtherInfo = updatePet.OtherInfo;

                context.Pets.Update(origPet);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePetById(long petId)
        {
            using (var context = new RofSchedulerContext())
            {
                var petToVax = await context.PetToVaccines.Where(v => v.PetId == petId).ToListAsync();

                var pet = await context.Pets.FirstOrDefaultAsync(p => p.Id == petId);

                if (pet == null)
                {
                    throw new ArgumentException($"No pet with Id: {petId} found.");
                }

                context.RemoveRange(petToVax);
                context.Remove(pet);

                await context.SaveChangesAsync();
            }
        }
    }
}
