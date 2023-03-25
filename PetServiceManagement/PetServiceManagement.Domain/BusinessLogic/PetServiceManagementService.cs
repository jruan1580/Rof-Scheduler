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
        private readonly IPetServiceRepository _petServiceRepository;

        public PetServiceManagementService(IPetServiceRepository petServiceRepository)
        {
            _petServiceRepository = petServiceRepository;
        }

        /// <summary>
        /// Enables searching by keyword
        /// Return results of the particular page of size "pageSize"
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<(List<PetService>, int)> GetPetServicesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _petServiceRepository.GetAllPetServicesByPageAndKeyword(page, pageSize, keyword);

            if (res.Item1.Count == 0)
            {
                return (new List<PetService>(), res.Item2);
            }

            var petServices = PetServiceMapper.ToDomainPetServices(res.Item1);            

            return (petServices, res.Item2);
        }

        /// <summary>
        /// Adds a new pet service if provided and name is filled in.
        /// </summary>
        /// <param name="petService"></param>
        /// <returns></returns>
        public async Task AddNewPetService(PetService petService)
        {
            ThrowArgumentExceptionIfValidationFails(petService);

            var petServiceEntity = PetServiceMapper.FromDomainPetService(petService);

            try
            {
                await _petServiceRepository.AddPetService(petServiceEntity);
            }
            catch(Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    throw new ArgumentException("Pet Service with same name already exists");
                }

                throw;
            }            
        }

        /// <summary>
        /// Updates a pet service's fields.
        /// </summary>
        /// <param name="petService"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdatePetService(PetService petService)
        {
            ThrowArgumentExceptionIfValidationFails(petService);

            var petServiceEntity = await _petServiceRepository.GetPetServiceById(petService.Id);

            if (petServiceEntity == null)
            {
                throw new ArgumentException($"Unable to locate existing service with id: {petService.Id}");
            }

            MergeUpdatedPetServiceToOriginalPetService(petServiceEntity, petService);
           
            await _petServiceRepository.UpdatePetService(petServiceEntity);
        }

        /// <summary>
        /// Removes all holiday rates tied to pet service first.
        /// Removes a pet service by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeletePetServiceById(short id)
        {
            await _petServiceRepository.DeletePetService(id);
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
