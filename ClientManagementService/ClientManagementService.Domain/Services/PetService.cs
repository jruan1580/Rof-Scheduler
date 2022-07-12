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

            var petExists = await _petRepository.PetAlreadyExists(newPet.OwnerId, newPet.BreedId, newPet.Name);
            if (petExists)
            {
                throw new ArgumentException($"This pet already exists under Owner with id: {newPet.OwnerId}.");
            }

            //var newClientEntity = ClientMapper.FromCoreClient(newPet);

            //await _clientRepository.CreateClient(newClientEntity);
        }
    }
}
