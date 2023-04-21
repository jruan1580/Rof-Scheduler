using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RofShared.Exceptions;
using DBPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;

namespace ClientManagementService.Domain.Services
{
    public class PetUpsertService : PetService, IPetUpsertService
    {
        private readonly IPetUpsertRepository _petUpsertRepository;

        public PetUpsertService(IPetUpsertRepository petUpsertRepository,
            IPetRetrievalRepository petRetrievalRepository,
            IPetToVaccinesRepository petToVaccinesRepository)
            : base(petRetrievalRepository, petToVaccinesRepository)
        {
            _petUpsertRepository = petUpsertRepository;
        }

        public async Task<long> AddPet(Pet newPet)
        {
            await ValidatePet(newPet, false);

            var newPetEntity = PetMapper.FromCorePet(newPet);

            var petId = await _petUpsertRepository.AddPet(newPetEntity);

            await AddVaccinesToNewPet(petId, newPet.Vaccines);

            return petId;
        }

        public async Task UpdatePet(Pet updatePet)
        {
            await ValidatePet(updatePet, true);

            var originalPet = await GetDbPetById(updatePet.Id);

            MergePetPropertiesForUpdate(originalPet, updatePet);

            await _petUpsertRepository.UpdatePet(originalPet);

            await MergePetVaccinesForUpdate(originalPet, updatePet);
        }

        public async Task DeletePetById(long petId)
        {
            await _petUpsertRepository.DeletePetById(petId);
        }

        private async Task ValidatePet(Pet pet, bool isUpdate)
        {
            ValidatePetProperties(pet, isUpdate);

            await ValidateIfPetIsDuplicate(pet.Id, pet.OwnerId, pet.Name, pet.BreedId);
        }

        private void ValidatePetProperties(Pet pet, bool isUpdate)
        {
            var validationErrors = (isUpdate) ? pet.GetValidationErrorsForUpdate() : pet.GetValidationErrorsForBothCreateOrUpdate();

            if (validationErrors.Count > 0)
            {
                var errorMessage = string.Join("\n", validationErrors);

                throw new ArgumentException(errorMessage);
            }
        }

        private async Task ValidateIfPetIsDuplicate(long petId, long ownerId, string name, short breedId)
        {
            var isDuplicate = await _petRetrievalRepository.DoesPetWithNameAndBreedExistUnderOwner(petId, ownerId, name, breedId);

            if (isDuplicate)
            {
                throw new ArgumentException("Pet with same name and breed already exist under this owner.");
            }
        }

        private async Task AddVaccinesToNewPet(long petId, List<VaccineStatus> vaccines)
        {
            var petsToVaccine = PetToVaccineMapper.ToPetToVaccine(petId, vaccines);

            await _petToVaccinesRepository.AddPetToVaccines(petsToVaccine);
        }

        private void MergePetPropertiesForUpdate(DBPet originalPet, Pet updatePet)
        {
            originalPet.Name = updatePet.Name;
            originalPet.Weight = updatePet.Weight;
            originalPet.Dob = updatePet.Dob;
            originalPet.OwnerId = updatePet.OwnerId;
            originalPet.BreedId = updatePet.BreedId;
            originalPet.PetTypeId = updatePet.PetTypeId;
            originalPet.OtherInfo = updatePet.OtherInfo;
        }

        private async Task MergePetVaccinesForUpdate(DBPet originalPet, Pet updatePet)
        {
            var origPetToVaccines = await GetDbPetToVaccineByPetId(originalPet.Id);

            foreach (var updatedPetToVaccine in updatePet.Vaccines)
            {
                var origPetToVaccine = origPetToVaccines.FirstOrDefault(o => o.Id == updatedPetToVaccine.PetToVaccineId);
                origPetToVaccine.Inoculated = updatedPetToVaccine.Inoculated;
            }

            await _petToVaccinesRepository.UpdatePetToVaccines(origPetToVaccines);
        }
    }
}
