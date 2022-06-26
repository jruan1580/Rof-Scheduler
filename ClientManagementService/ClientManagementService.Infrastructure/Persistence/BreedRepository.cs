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
        Task DeleteBreedById(long id);
        Task<List<Breed>> GetAllBreedsByType(string type);
        Task UpdateBreed(Breed updateBreed);
    }

    public class BreedRepository : IBreedRepository
    {
        public async Task<List<Breed>> GetAllBreedsByType(string type)
        {
            using (var context = new RofSchedulerContext())
            {
                var breedList = new List<Breed>();

                type = type.ToLower();

                breedList = await context.Breeds.Where(b => (b.Type.ToLower().Contains(type))).ToListAsync();
                
                return breedList;
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

        public async Task DeleteBreedById(long id)
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
