using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IBreedRepository
    {
        Task AddBreed(Breed newBreed);
        Task DeleteBreedById(short id);
        Task<List<Breed>> GetAllBreedsByType(string type);
        Task<List<Breed>> GetBreedsByBreedIds(List<short> breedIds);
        Task<Breed> GetBreedById(short id);
        Task UpdateBreed(Breed updateBreed);
    }

    public class BreedRepository : IBreedRepository
    {
        public async Task<List<Breed>> GetAllBreedsByType(string type)
        {
            using (var context = new RofSchedulerContext())
            {
                var petType = await context.PetTypes.FirstOrDefaultAsync(t => t.PetTypeName.ToLower() == type.ToLower());

                //unable to find pet type, therefore no breeds
                //return empty list
                if (petType == null)
                {
                    return new List<Breed>();
                }

                return await context.Breeds.Where(b => b.PetTypeId == petType.Id).ToListAsync();                
            }
        }

        public async Task<List<Breed>> GetBreedsByBreedIds(List<short> breedIds)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Breeds.Where(b => breedIds.Any(id => id == b.Id)).ToListAsync();
            }
        }

        public async Task<Breed> GetBreedById(short id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Breeds.FirstOrDefaultAsync(b => b.Id == id);
            }
        }

        public async Task AddBreed(Breed newBreed)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Breeds.Add(newBreed);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateBreed(Breed updateBreed)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Breeds.Update(updateBreed);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteBreedById(short id)
        {
            using (var context = new RofSchedulerContext())
            {
                var breed = await context.Breeds.FirstOrDefaultAsync(b => b.Id == id);

                if (breed == null)
                {
                    throw new ArgumentException($"No breed with Id: {id} found.");
                }

                context.Remove(breed);

                await context.SaveChangesAsync();
            }
        }
    }
}
