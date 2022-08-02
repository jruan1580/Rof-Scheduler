using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IDropdownService
    {
        Task<List<PetType>> GetPetTypes();
        Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId);
    }

    public class DropdownService : IDropdownService
    {
        private readonly IPetRepository _petRepository;
        private readonly IPetToVaccinesRepository _petToVaccineRepository;

        public DropdownService(IPetRepository petRepository, IPetToVaccinesRepository petToVaccineToRepository)
        {
            _petRepository = petRepository;
            _petToVaccineRepository = petToVaccineToRepository;
        }

        /// <summary>
        /// Returns a list of pet types.
        /// Function is used mainaly to populate drop down list
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a list of vaccines by pet type.
        /// Function is used mainaly to populate drop down list
        /// </summary>
        /// <param name="petTypeId"></param>
        /// <returns></returns>
        public async Task<List<Vaccine>> GetVaccinesByPetType(short petTypeId)
        {
            var vaccines = new List<Vaccine>();

            foreach (var vax in await _petToVaccineRepository.GetVaccinesByPetType(petTypeId))
            {
                vaccines.Add(PetToVaccineMapper.ToCoreVax(vax));
            }

            return vaccines;
        }
    }
}
