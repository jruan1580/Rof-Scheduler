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

            await _petServiceRepository.AddPetService(petServiceEntity);
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
            //we cannot simply delete pet service because holidayRates has a foreign key to pet services.
            //we need to delete all holiday rate set on pet service first before removing pet services.


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
        }
    }
}
