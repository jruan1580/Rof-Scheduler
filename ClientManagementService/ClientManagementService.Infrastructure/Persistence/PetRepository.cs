using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IPetRepository
    {
        Task AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task<List<Pet>> GetAllPets();
        Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter);
        Task<List<Pet>> GetPetsByClientId(long clientId);
        Task UpdatePet(Pet updatePet);
    }

    public class PetRepository : IPetRepository
    {
        public async Task AddPet(Pet newPet)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Pets.Add(newPet);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Pet>> GetPetsByClientId(long clientId)
        {
            using (var context = new RofSchedulerContext())
            {
                var pets = await context.Pets.Include(p => p.Owner).Where(p => p.OwnerId == clientId).ToListAsync();

                var petList = new List<Pet>();

                foreach (var pet in pets)
                {
                    petList.Add(pet);
                }

                return petList;
            }
        }

        public async Task<List<Pet>> GetAllPets()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Pets.ToListAsync();
            }
        }

        public async Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter)
        {
            using (var context = new RofSchedulerContext())
            {
                if (filter.Filter == GetPetFilterEnum.Id)
                {
                    return await context.Pets.FirstOrDefaultAsync(p => p.Id == Convert.ToInt64(filter.Value));
                }
                else if (filter.Filter == GetPetFilterEnum.Name)
                {
                    return await context.Pets.FirstOrDefaultAsync(p => p.Name.ToLower().Equals(Convert.ToString(filter.Value).ToLower()));
                }
                else
                {
                    throw new ArgumentException("Invalid Filter Type.");
                }
            }
        }

        public async Task UpdatePet(Pet updatePet)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Pets.Update(updatePet);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePetById(long petId)
        {
            using (var context = new RofSchedulerContext())
            {
                var pet = await context.Pets.FirstOrDefaultAsync(p => p.Id == petId);

                if (pet == null)
                {
                    throw new ArgumentException($"No pet with Id: {petId} found.");
                }

                context.Remove(pet);

                await context.SaveChangesAsync();
            }
        }
    }
}
