using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RofShared.Exceptions;

namespace ClientManagementService.Domain.Services
{
    public interface IPetService
    {
        Task<long> AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword);
        Task<Pet> GetPetById(long petId);
        Task<Pet> GetPetByName(string name);
        Task<PetsWithTotalPage> GetPetsByClientIdAndKeyword(long clientId, int page, int offset, string keyword);
        Task UpdatePet(Pet updatePet);
        Task<List<VaccineStatus>> GetVaccinesByPetId(long petId);
    }

    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IPetRetrievalRepository _petRetrievalRepository;
        private readonly IPetToVaccinesRepository _petToVaccinesRepository;

        public PetService(IPetRepository petRepository, 
            IPetRetrievalRepository petRetrievalRepository,
            IPetToVaccinesRepository petToVaccinesRepository)
        {
            _petRepository = petRepository;

            _petRetrievalRepository = petRetrievalRepository;

            _petToVaccinesRepository = petToVaccinesRepository;
        }

        public async Task<long> AddPet(Pet newPet)
        {
            var invalidErrs = newPet.IsValidPetToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var petExists = await _petRetrievalRepository.DoesPetWithNameAndBreedExistUnderOwner(0, newPet.OwnerId, newPet.Name, newPet.BreedId);
            if (petExists)
            {
                throw new ArgumentException($"This pet already exists under Owner with id: {newPet.OwnerId}.");
            }

            var newPetEntity = PetMapper.FromCorePet(newPet);            

            var petId = await _petRepository.AddPet(newPetEntity);

            //add their vaccines after
            var petsToVaccine = PetToVaccineMapper.ToPetToVaccine(petId, newPet.Vaccines);

            await _petToVaccinesRepository.AddPetToVaccines(petsToVaccine);

            return petId;
        }

        public async Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword)
        {
            var result = await _petRetrievalRepository.GetAllPetsByKeyword(page, offset, keyword);
            var pets = result.Item1;
            var totalPages = result.Item2;

            if (pets == null || pets.Count == 0)
            {
                return new PetsWithTotalPage(new List<Pet>(), 0);
            }                  

            //no need to map vaccine status as we will not be displaying vaccines when getting ALL pets to show in table format
            //so pass in null for PetToVaccines parameter
            return new PetsWithTotalPage(pets.Select(p => PetMapper.ToCorePet(p, null)).ToList(), totalPages);
        }

        public async Task<Pet> GetPetById(long petId)
        {
            var pet = await _petRetrievalRepository.GetPetByFilter(new GetPetFilterModel<long>(GetPetFilterEnum.Id, petId));

            if (pet == null)
            {
                throw new EntityNotFoundException("Pet was not found");
            }

            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(pet.Id);

            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                throw new EntityNotFoundException("Pet had no records of vaccines");
            }

            return PetMapper.ToCorePet(pet, petToVaccines);
        }

        public async Task<Pet> GetPetByName(string name)
        {
            var pet = await _petRetrievalRepository.GetPetByFilter(new GetPetFilterModel<string>(GetPetFilterEnum.Name, name));

            if (pet == null)
            {
                throw new EntityNotFoundException("Pet was not found");
            }

            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(pet.Id);

            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                throw new EntityNotFoundException("Pet had no records of vaccines");
            }

            return PetMapper.ToCorePet(pet, petToVaccines);
        }

        public async Task<PetsWithTotalPage> GetPetsByClientIdAndKeyword(long clientId, int page, int offset, string keyword)
        {
            var result = await _petRetrievalRepository.GetPetsByClientIdAndKeyword(clientId, page, offset, keyword);
            var pets = result.Item1;
            var totalPages = result.Item2;

            if (pets == null || pets.Count == 0)
            {
                return new PetsWithTotalPage(new List<Pet>(), 0);
            }

            return new PetsWithTotalPage(pets.Select(p => PetMapper.ToCorePet(p, null)).ToList(), totalPages);
        }

        public async Task UpdatePet(Pet updatePet)
        {
            var invalidErrs = updatePet.IsValidPetToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var petExists = await _petRetrievalRepository.DoesPetWithNameAndBreedExistUnderOwner(updatePet.Id, updatePet.OwnerId, updatePet.Name, updatePet.BreedId);
            if (petExists)
            {
                throw new ArgumentException($"Pet with same name and breed already exist under this owner id {updatePet.OwnerId}");
            }

            var origPet = await _petRetrievalRepository.GetPetByFilter(new GetPetFilterModel<long>(GetPetFilterEnum.Id, updatePet.Id));
            if (origPet == null)
            {
                throw new EntityNotFoundException("Pet was not found. Failed to update.");
            }
            
            var petEntity = PetMapper.FromCorePet(updatePet);

            await _petRepository.UpdatePet(petEntity);

            var origPetToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(origPet.Id);
            foreach(var updatedPetToVaccine in updatePet.Vaccines)
            {
                var origPetToVaccine = origPetToVaccines.FirstOrDefault(o => o.Id == updatedPetToVaccine.PetToVaccineId);
                origPetToVaccine.Inoculated = updatedPetToVaccine.Inoculated;
            }         

            await _petToVaccinesRepository.UpdatePetToVaccines(origPetToVaccines);
        }

        public async Task DeletePetById(long petId)
        {
            await _petRepository.DeletePetById(petId);
        }

        public async Task<List<VaccineStatus>> GetVaccinesByPetId(long petId)
        {
            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(petId); //list of petToVax with Id, PetId, VaxId, and Innoculated

            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                throw new EntityNotFoundException("Pet had no records of vaccines");
            }            

            return PetToVaccineMapper.ToVaccineStatus(petToVaccines);
        }
    }
}
