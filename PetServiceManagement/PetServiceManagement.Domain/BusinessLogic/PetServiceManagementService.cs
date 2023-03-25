using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IPetServiceManagementService
    {
        Task AddNewPetService(PetService petService);
        Task DeletePetServiceById(short id);
        Task<(List<PetService>, int)> GetPetServicesByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task UpdatePetService(PetService petService);
    }

    public class PetServiceManagementService : IPetServiceManagementService
    {
        private readonly IPetServiceRepository _petServiceRepository;
        private readonly HashSet<string> _supportedTimeUnits;

        public PetServiceManagementService(IPetServiceRepository petServiceRepository)
        {
            _petServiceRepository = petServiceRepository;

            _supportedTimeUnits = new HashSet<string>() { TimeUnits.SECONDS, TimeUnits.MINUTES, TimeUnits.HOURS };
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

            var petServices = new List<PetService>();
            res.Item1.ForEach(pet => petServices.Add(PetServiceMapper.ToDomainPetService(pet)));

            return (petServices, res.Item2);
        }

        /// <summary>
        /// Adds a new pet service if provided and name is filled in.
        /// </summary>
        /// <param name="petService"></param>
        /// <returns></returns>
        public async Task AddNewPetService(PetService petService)
        {
            RunValidation(petService);

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
            RunValidation(petService);

            var petServiceEntity = await _petServiceRepository.GetPetServiceById(petService.Id);

            if (petServiceEntity == null)
            {
                throw new ArgumentException($"Unable to locate existing service with id: {petService.Id}");
            }

            petServiceEntity.ServiceName = petService.Name;
            petServiceEntity.Price = petService.Price;
            petServiceEntity.Description = petService.Description;
            petServiceEntity.EmployeeRate = petService.EmployeeRate;
            petServiceEntity.Duration = petService.Duration;
            petServiceEntity.TimeUnit = petService.TimeUnit;

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

        private void RunValidation(PetService petService)
        {
            if (petService == null)
            {
                throw new ArgumentException("New service not provided");
            }

            //only name is important
            if (string.IsNullOrEmpty(petService.Name))
            {
                throw new ArgumentException("Pet service's name is not provided");
            }

            if (petService.Price < 0)
            {
                throw new ArgumentException("Pet service rate must be greater than 0");
            }

            if (petService.EmployeeRate > 100 || petService.EmployeeRate < 0)
            {
                throw new ArgumentException("Employee rate should be between 0 and 100");
            }

            if (petService.Duration < 0)
            {
                throw new ArgumentException("Pet Service Duration must be greater than 0");
            }

            if (!_supportedTimeUnits.Contains(petService.TimeUnit))
            {
                throw new ArgumentException("Time Unit not supported.");
            }
        }
    }
}
