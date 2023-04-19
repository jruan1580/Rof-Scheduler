using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public class PetRetrievalService : PetBaseService
    {
        public PetRetrievalService(IPetRetrievalRepository petRetrievalRepository,
            IPetToVaccinesRepository petToVaccinesRepository)
            : base(petRetrievalRepository, petToVaccinesRepository) { }

        public async Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword)
        {
            var result = await _petRetrievalRepository.GetAllPetsByKeyword(page, offset, keyword);

            var pets = result.Item1;
            var totalPages = result.Item2;

            //no need to map vaccine status as we will not be displaying vaccines when getting ALL pets to show in table format
            //so pass in null for PetToVaccines parameter
            var corePets = pets.Select(p => PetMapper.ToCorePet(p, null)).ToList();

            return new PetsWithTotalPage(corePets, totalPages);
        }

        public async Task<PetsWithTotalPage> GetPetsByClientIdAndKeyword(long clientId, int page, int offset, string keyword)
        {
            var result = await _petRetrievalRepository.GetPetsByClientIdAndKeyword(clientId, page, offset, keyword);

            var pets = result.Item1;
            var totalPages = result.Item2;

            var corePets = pets.Select(p => PetMapper.ToCorePet(p, null)).ToList();

            return new PetsWithTotalPage(corePets, totalPages);
        }

        public async Task<Pet> GetPetById(long petId)
        {
            var pet = await GetDbPetById(petId);

            var petToVaccines = await GetDbPetToVaccineByPetId(pet.Id);

            return PetMapper.ToCorePet(pet, petToVaccines);
        }

        public async Task<Pet> GetPetByName(string name)
        {
            var pet = await GetDbPetByName(name);

            var petToVaccines = await GetDbPetToVaccineByPetId(pet.Id);

            return PetMapper.ToCorePet(pet, petToVaccines);
        }        
    }
}
