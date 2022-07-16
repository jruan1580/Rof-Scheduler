using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public class PetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task AddPet(Pet newPet)
        {
            var invalidErrs = newPet.IsValidPetToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var newPetEntity = PetMapper.FromCorePet(newPet);

            await _petRepository.AddPet(newPetEntity);
        }


        //Task<(List<Pet>, int)> GetAllPetsByKeyword(int page = 1, int offset = 10, string keyword = "");
        //Task<Pet> GetPetByFilter<T>(GetPetFilterModel<T> filter);
        //Task<List<Pet>> GetPetsByClientId(long clientId);
        //Task UpdatePet(Pet updatePet);
        //Task DeletePetById(long petId);
    }
}
