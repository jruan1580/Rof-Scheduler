using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IPetService
    {
        Task AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword);
        Task<Pet> GetPetById(long petId);
        Task<Pet> GetPetByName(string name);
        Task<List<Pet>> GetPetsByClientId(long clientId);
        Task<List<PetType>> GetPetTypes();
        Task UpdatePet(Pet updatePet);
    }

    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IPetToVaccinesRepository _petToVaccinesRepository;

        public PetService(IPetRepository petRepository, IPetToVaccinesRepository petToVaccinesRepository)
        {
            _petRepository = petRepository;

            _petToVaccinesRepository = petToVaccinesRepository;
        }

        public async Task AddPet(Pet newPet)
        {
            var invalidErrs = newPet.IsValidPetToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var petExists = await _petRepository.PetAlreadyExists(newPet.OwnerId, newPet.Name);
            if (petExists)
            {
                throw new ArgumentException($"This pet already exists under Owner with id: {newPet.OwnerId}.");
            }

            var newPetEntity = PetMapper.FromCorePet(newPet);            

            var petId = await _petRepository.AddPet(newPetEntity);

            //add their vaccines after
            var petsToVaccine = PetToVaccineMapper.ToPetToVaccine(petId, newPet.Vaccines);

            await _petToVaccinesRepository.AddPetToVaccines(petsToVaccine);
        }

        public async Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword)
        {
            var result = await _petRepository.GetAllPetsByKeyword(page, offset, keyword);
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
            var pet = await _petRepository.GetPetByFilter(new GetPetFilterModel<long>(GetPetFilterEnum.Id, petId));

            if (pet == null)
            {
                throw new PetNotFoundException();
            }

            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(pet.Id);

            if (petToVaccines == null || petToVaccines.Count == 0)
            {

            }

            return PetMapper.ToCorePet(pet, petToVaccines);
        }

        public async Task<Pet> GetPetByName(string name)
        {
            var pet = await _petRepository.GetPetByFilter(new GetPetFilterModel<string>(GetPetFilterEnum.Name, name));

            if (pet == null)
            {
                throw new PetNotFoundException();
            }

            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(pet.Id);

            if (petToVaccines == null || petToVaccines.Count == 0)
            {

            }

            return PetMapper.ToCorePet(pet, petToVaccines);
        }

        public async Task<List<Pet>> GetPetsByClientId(long clientId)
        {
            var dbPets = await _petRepository.GetPetsByClientId(clientId);

            var pets = new List<Pet>();

            if (dbPets.Count == 0)
            {
                return pets;
            }
         
            foreach (var pet in dbPets)
            {
                //when displaying a list of pets in table, we will not display vaccines.
                //otw, table will be very big and congested
                pets.Add(PetMapper.ToCorePet(pet, null));
            }

            return pets;
        }

        public async Task<List<PetType>> GetPetTypes()
        {
            var pt = await _petRepository.GetAllPetTypes();

            var types = new List<PetType>();
            foreach (var petType in pt)
            {
                types.Add(PetMapper.ToCorePetType(petType));
            }

            return types;
        }

        public async Task UpdatePet(Pet updatePet)
        {
            var invalidErrs = updatePet.IsValidPetToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var petExists = await _petRepository.PetAlreadyExists(updatePet.OwnerId, updatePet.Name);
            if (petExists)
            {
                throw new ArgumentException($"Pet with same name and breed already exist under this owner id {updatePet.OwnerId}");
            }

            var origPet = await _petRepository.GetPetByFilter(new GetPetFilterModel<long>(GetPetFilterEnum.Id, updatePet.Id));
            if (origPet == null)
            {
                throw new PetNotFoundException();
            }

            origPet.Name = updatePet.Name;
            origPet.Weight = updatePet.Weight;
            origPet.Dob = updatePet.Dob;
            origPet.BreedId = updatePet.BreedId;
            origPet.OwnerId = updatePet.OwnerId;
            origPet.OtherInfo = updatePet.OtherInfo;

            await _petRepository.UpdatePet(origPet);
        }

        public async Task DeletePetById(long petId)
        {
            await _petRepository.DeletePetById(petId);
        }       
    }
}
