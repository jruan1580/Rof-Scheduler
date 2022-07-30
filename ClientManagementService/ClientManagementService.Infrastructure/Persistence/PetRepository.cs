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
        Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter);
        Task<List<Pet>> GetPetsByClientId(long clientId);
        Task UpdatePet(Pet updatePet);
        Task<bool> PetAlreadyExists(long ownerId, long breedId, string name);
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
                var pets = await context.Pets.Where(p => p.OwnerId == clientId).ToListAsync();

                var petList = new List<Pet>();

                foreach (var pet in pets)
                {
                    petList.Add(pet);
                }

                return petList;
            }
        }

        public async Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using (var context = new RofSchedulerContext())
            {
                var skip = (page - 1) * offset;
                IQueryable<Pet> pet = context.Pets;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    pet = context.Pets.Where(p => (p.Name.ToLower().Contains(keyword)));
                }

                var countByCriteria = await pet.CountAsync();
                var fullPages = countByCriteria / offset;
                var remaining = countByCriteria % offset;
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages;

                if (page > totalPages)
                {
                    throw new ArgumentException("No more pets.");
                }

                var result = await pet.OrderByDescending(p => p.Id).Skip(skip).Take(offset).ToListAsync();

                return (result, totalPages);
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

        public async Task<bool> PetAlreadyExists(long ownerId, long breedId, string name)
        {
            using (var context = new RofSchedulerContext())
            {
                name = name.ToLower();

                return await context.Pets.AnyAsync(p => p.Name.ToLower().Equals(name) && p.OwnerId.Equals(ownerId) && p.BreedId.Equals(breedId));
            }
        }
    }
}
