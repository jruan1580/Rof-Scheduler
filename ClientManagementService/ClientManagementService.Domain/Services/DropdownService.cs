using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IDropdownService
    {
        Task<List<Client>> GetClients();
        Task<List<PetType>> GetPetTypes();
        Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId);
        Task<List<Breed>> GetBreedsByPetType(short petTypeId);
        Task<List<Pet>> GetPets();
    }

    public class DropdownService : IDropdownService
    {
        private readonly IClientRetrievalRepository _clientRetrievalRepository;
        private readonly IPetRetrievalRepository _petRetrievalRepository;
        private readonly IPetToVaccinesRepository _petToVaccineRepository;

        public DropdownService(IClientRetrievalRepository clientRetrievalRepository,
            IPetRetrievalRepository petRetrievalRepository, 
            IPetToVaccinesRepository petToVaccineToRepository)
        {
            _clientRetrievalRepository = clientRetrievalRepository;
            _petRetrievalRepository = petRetrievalRepository;
            _petToVaccineRepository = petToVaccineToRepository;
        }

        public async Task<List<PetType>> GetPetTypes()
        {
            var pt = await _petRetrievalRepository.GetPetTypesForDropdown();

            var types = new List<PetType>();
            foreach (var petType in pt)
            {
                types.Add(PetMapper.ToCorePetType(petType));
            }

            return types;
        }

        public async Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId)
        {
            var vaccines = new List<Vaccine>();

            foreach (var vax in await _petToVaccineRepository.GetVaccinesByPetType(petTypeId))
            {
                vaccines.Add(PetToVaccineMapper.ToCoreVax(vax));
            }

            return vaccines;
        }

        public async Task<List<Breed>> GetBreedsByPetType(short petTypeId)
        {
            var breeds = new List<Breed>();

            foreach (var breed in await _petRetrievalRepository.GetBreedsByPetTypeIdForDropdown(petTypeId))
            {
                breeds.Add(PetMapper.ToCoreBreed(breed));
            }

            return breeds;
        }

        public async Task<List<Client>> GetClients()
        {
            var  clients = new List<Client>();

            foreach(var client in await _clientRetrievalRepository.GetClientsForDropdown())
            {
                clients.Add(ClientMapper.ToCoreClient(client));
            }

            return clients;
        }

        public async Task<List<Pet>> GetPets()
        {
            var pets = new List<Pet>();

            foreach (var pet in await _petRetrievalRepository.GetPetsForDropdown())
            {
                pets.Add(PetMapper.ToCorePetDropDown(pet));
            }

            return pets;
        }
    }
}
