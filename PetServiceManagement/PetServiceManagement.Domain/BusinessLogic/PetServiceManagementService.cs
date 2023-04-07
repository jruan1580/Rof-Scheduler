using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class PetServiceManagementService : IPetServiceManagementService
    {
        private readonly IPetServiceRetrievalRepository _petServiceRetrievalRepository;
        private readonly IPetServiceUpsertRepository _petServiceUpsertRepository;

        public PetServiceManagementService(IPetServiceRetrievalRepository petServiceRetrievalRepository,
            IPetServiceUpsertRepository petServiceUpsertRepository)
        {
            _petServiceRetrievalRepository = petServiceRetrievalRepository;

            _petServiceUpsertRepository = petServiceUpsertRepository;
        }

        public async Task<(List<PetService>, int)> GetPetServicesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _petServiceRetrievalRepository.GetAllPetServicesByPageAndKeyword(page, pageSize, keyword);

            if (res.Item1.Count == 0)
            {
                return (new List<PetService>(), res.Item2);
            }

            var petServices = PetServiceMapper.ToDomainPetServices(res.Item1);            

            return (petServices, res.Item2);
        }

        public async Task AddNewPetService(PetService petService)
        {
            ThrowArgumentExceptionIfValidationFails(petService);

            var petServiceEntity = PetServiceMapper.FromDomainPetService(petService);

            try
            {
                await _petServiceUpsertRepository.AddPetService(petServiceEntity);
            }
            catch (DbUpdateException dbEx)
            {
                DbExceptionHandler.HandleDbUpdateException(dbEx, "Pet Service");
            }            
        }

        public async Task UpdatePetService(PetService petService)
        {
            ThrowArgumentExceptionIfValidationFails(petService);

            var petServiceEntity = await _petServiceRetrievalRepository.GetPetServiceById(petService.Id);

            if (petServiceEntity == null)
            {
                throw new ArgumentException($"Unable to locate existing service with id: {petService.Id}");
            }

            MergeUpdatedPetServiceToOriginalPetService(petServiceEntity, petService);
           
            await _petServiceUpsertRepository.UpdatePetService(petServiceEntity);
        }

        public async Task DeletePetServiceById(short id)
        {
            await _petServiceUpsertRepository.DeletePetService(id);
        }

        private void MergeUpdatedPetServiceToOriginalPetService(PetServices original, PetService updated)
        {
            original.ServiceName = updated.Name;
            original.Price = updated.Price;
            original.Description = updated.Description;
            original.EmployeeRate = updated.EmployeeRate;
            original.Duration = updated.Duration;
            original.TimeUnit = updated.TimeUnit;
        }

        private void ThrowArgumentExceptionIfValidationFails(PetService petService)
        {
            if (petService == null)
            {
                throw new ArgumentException("New service not provided");
            }

            var validationFailures = petService.GetValidationFailures();

            if (string.IsNullOrEmpty(validationFailures))
            {
                return;
            }

            throw new ArgumentException(validationFailures);
        }
    }
}
