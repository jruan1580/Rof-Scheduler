﻿using ClientManagementService.Infrastructure.Persistence.Entities;
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
        Task<long> AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task<List<PetType>> GetAllPetTypes();
        Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter);
        Task<(List<Pet>, int)> GetPetsByClientIdAndKeyword(long clientId, int page = 1, int offset = 10, string keyword = "");
        Task<List<Breed>> GetPetBreedByPetTypeId(short petTypeId);
        Task UpdatePet(Pet updatePet);
        Task<bool> PetAlreadyExists(long petId, long ownerId, string name);
        Task<List<Pet>> GetPetsForDropdown();
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

        public async Task<List<Pet>> GetPetsForDropdown()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Pets.Select(p => new Pet() { Id = p.Id, Name = p.Name }).ToListAsync();
            }
        }

        /// <summary>
        /// Will be called when we want to retrieve a list of pet types for drop down list
        /// </summary>
        /// <returns></returns>
        public async Task<List<PetType>> GetAllPetTypes()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.PetTypes.ToListAsync();
            }
        }

        public async Task<(List<Pet>, int)> GetPetsByClientIdAndKeyword(long clientId, int page = 1, int offset = 10, string keyword ="")
        {
            using (var context = new RofSchedulerContext())
            {
                var skip = (page - 1) * offset;
                IQueryable<Pet> pets = context.Pets.Where(p => p.OwnerId == clientId);

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    pets = context.Pets.Where(p => (p.Name.ToLower().Contains(keyword)));
                }

                var countByCriteria = await pets.CountAsync();

                if (countByCriteria == 0)
                {
                    return (new List<Pet>(), 0);
                }

                var fullPages = countByCriteria / offset;
                var remaining = countByCriteria % offset;
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages;

                if (page > totalPages)
                {
                    throw new ArgumentException("No more pets.");
                }

                var res = await pets.Skip(skip).Take(offset).ToListAsync();

                await PopulateBreedOwnerAndPetType(context, res);

                return (res, totalPages);
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

                if (countByCriteria == 0)
                {
                    return (new List<Pet>(), 0);
                }

                var fullPages = countByCriteria / offset;
                var remaining = countByCriteria % offset;
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages;

                if (page > totalPages)
                {
                    throw new ArgumentException("No more pets.");
                }

                var result = await pet.OrderByDescending(p => p.Id).Skip(skip).Take(offset).ToListAsync();

                await PopulateBreedOwnerAndPetType(context, result);
    
                return (result, totalPages);
            }
        }

        public async Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter)
        {
            using (var context = new RofSchedulerContext())
            {
                Pet pet = null;

                if (filter.Filter == GetPetFilterEnum.Id)
                {
                    pet = await context.Pets.FirstOrDefaultAsync(p => p.Id == Convert.ToInt64(filter.Value));
                }
                else if (filter.Filter == GetPetFilterEnum.Name)
                {
                    pet = await context.Pets.FirstOrDefaultAsync(p => p.Name.ToLower().Equals(Convert.ToString(filter.Value).ToLower()));
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
        }

        /// <summary>
        /// Will call to retrieve a list of breeds after pet type has been selected
        /// </summary>
        /// <param name="petTypeId"></param>
        /// <returns></returns>
        public async Task<List<Breed>> GetPetBreedByPetTypeId(short petTypeId)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Breeds.Where(b => b.PetTypeId == petTypeId).ToListAsync();
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

        public async Task<bool> PetAlreadyExists(long petId, long ownerId, string name)
        {
            using (var context = new RofSchedulerContext())
            {
                name = name.ToLower();

                return await context.Pets.AnyAsync(p => p.Id != petId && p.Name.ToLower().Equals(name) && p.OwnerId.Equals(ownerId));
            }
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
    }
}
