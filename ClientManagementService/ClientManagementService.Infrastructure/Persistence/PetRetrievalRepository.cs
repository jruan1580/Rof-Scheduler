using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using Microsoft.EntityFrameworkCore;
using RofShared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IPetRetrievalRepository
    {
        Task<bool> DoesPetExistByNameAndOwner(long petId, long ownerId, string name);
        Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<List<Breed>> GetBreedsByPetTypeIdForDropdown(short petTypeId);
        Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter);
        Task<(List<Pet>, int)> GetPetsByClientIdAndKeyword(long clientId, int page = 1, int offset = 10, string keyword = "");
        Task<List<Pet>> GetPetsForDropdown();
        Task<List<PetType>> GetPetTypesForDropdown();
    }

    public class PetRetrievalRepository : IPetRetrievalRepository
    {
        public async Task<List<Pet>> GetPetsForDropdown()
        {
            using var context = new RofSchedulerContext();

            return await context.Pets.Select(p => new Pet() { Id = p.Id, Name = p.Name }).ToListAsync();
        }

        public async Task<List<PetType>> GetPetTypesForDropdown()
        {
            using var context = new RofSchedulerContext();

            return await context.PetTypes.ToListAsync();
        }

        public async Task<List<Breed>> GetBreedsByPetTypeIdForDropdown(short petTypeId)
        {
            using var context = new RofSchedulerContext();

            return await context.Breeds.Where(b => b.PetTypeId == petTypeId).ToListAsync();
        }

        public async Task<(List<Pet>, int)> GetPetsByClientIdAndKeyword(long clientId, int page = 1, int offset = 10, string keyword = "")
        {
            using var context = new RofSchedulerContext();

            var skip = (page - 1) * offset;

            IQueryable<Pet> pets = context.Pets.Where(p => p.OwnerId == clientId);

            var petList = FilterByKeyword(context, pets, keyword?.Trim()?.ToLower());

            var countByCriteria = await petList.CountAsync();

            var totalPages = DatabaseUtilities.GetTotalPages(countByCriteria, offset, page);

            var result = await petList.OrderByDescending(p => p.Id).Skip(skip).Take(offset).ToListAsync();

            await PopulateBreedOwnerAndPetType(context, result);

            return (result, totalPages);
        }

        public async Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using var context = new RofSchedulerContext();

            var skip = (page - 1) * offset;

            IQueryable<Pet> pets = context.Pets;

            var petList = FilterByKeyword(context, pets, keyword?.Trim()?.ToLower());

            var countByCriteria = await petList.CountAsync();

            var totalPages = DatabaseUtilities.GetTotalPages(countByCriteria, offset, page);

            var result = await petList.OrderByDescending(p => p.Id).Skip(skip).Take(offset).ToListAsync();

            await PopulateBreedOwnerAndPetType(context, result);

            return (result, totalPages);
        }

        public async Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter)
        {
            using var context = new RofSchedulerContext();

            Pet pet = null;

            if (filter.Filter == GetPetFilterEnum.Id)
            {
                var val = Convert.ToInt64(filter.Value);

                pet = await context.Pets.FirstOrDefaultAsync(p => p.Id == val);
            }
            else if (filter.Filter == GetPetFilterEnum.Name)
            {
                var name = Convert.ToString(filter.Value).ToLower();

                pet = await context.Pets.FirstOrDefaultAsync(p => p.Name.ToLower().Equals(name));
            }
            else
            {
                throw new ArgumentException("Invalid Filter Type.");
            }

            if (pet == null)
            {
                return null;
            }

            pet.Owner = await context.Clients.FirstOrDefaultAsync(c => c.Id == pet.OwnerId);
            pet.Breed = await context.Breeds.FirstOrDefaultAsync(b => b.Id == pet.BreedId);
            pet.PetType = await context.PetTypes.FirstOrDefaultAsync(pt => pt.Id == pet.PetTypeId);

            return pet;
        }

        public async Task<bool> DoesPetExistByNameAndOwner(long petId, long ownerId, string name)
        {
            using var context = new RofSchedulerContext();

            name = name.ToLower();

            return await context.Pets.AnyAsync(p => p.Id != petId
                && p.Name.ToLower().Equals(name)
                && p.OwnerId.Equals(ownerId));
        }

        private async Task PopulateBreedOwnerAndPetType(RofSchedulerContext context, List<Pet> pets)
        {
            var clientIds = pets.Select(p => p.OwnerId).Distinct().ToList();
            var clients = await context.Clients.Where(c => clientIds.Any(id => c.Id == id)).ToListAsync();

            var breedIds = pets.Select(p => p.BreedId).Distinct().ToList();
            var breeds = await context.Breeds.Where(b => breedIds.Contains(b.Id)).ToListAsync();

            var petTypeIds = pets.Select(p => p.PetTypeId).Distinct().ToList();
            var petTypes = await context.PetTypes.Where(pt => petTypeIds.Contains(pt.Id)).ToListAsync();

            foreach (var pet in pets)
            {
                pet.Owner = clients.First(c => c.Id == pet.OwnerId);
                pet.Breed = breeds.First(b => b.Id == pet.BreedId);
                pet.PetType = petTypes.First(pt => pt.Id == pet.PetTypeId);
            }
        }

        private IQueryable<Pet> FilterByKeyword(RofSchedulerContext context, IQueryable<Pet> pets, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return pets;
            }

            return pets.Where(p => (p.Name.ToLower().Contains(keyword))
                || (p.PetType.PetTypeName.ToLower().Contains(keyword))
                || (p.Breed.BreedName.ToLower().Contains(keyword))
                || (p.Owner.FirstName.ToLower().Contains(keyword))
                || (p.Owner.LastName.ToLower().Contains(keyword)));
        }
    }
}
