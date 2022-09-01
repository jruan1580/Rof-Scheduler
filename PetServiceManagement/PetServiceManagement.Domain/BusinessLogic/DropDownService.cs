using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IDropDownService
    {
        Task<List<T>> GetDropdownForType<T>();
    }

    public class DropDownService : IDropDownService
    {
        private readonly IPetServiceRepository _petServiceRepository;
        private readonly IHolidayAndRatesRepository _holidayAndRatesRepository;

        public DropDownService(IPetServiceRepository petServiceRepository,
            IHolidayAndRatesRepository holidayAndRatesRepository)
        {
            _petServiceRepository = petServiceRepository;
            _holidayAndRatesRepository = holidayAndRatesRepository;
        }

        /// <summary>
        /// Depending on generic type, return values for dropdown
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<T>> GetDropdownForType<T>()
        {
            var result = new List<T>();
            if (typeof(T) == typeof(Holiday))
            {
                var holidays = await _holidayAndRatesRepository.GetAllHolidaysForDropdowns();
                holidays.ForEach(h =>
                {
                    var holiday = (T)Convert.ChangeType(HolidayMapper.ToHolidayDomain(h), typeof(T));
                    result.Add(holiday);
                });
            }
            else if (typeof(T) == typeof(PetService))
            {
                var petServices = await _petServiceRepository.GetAllPetServicesForDropdown();
                petServices.ForEach(pet =>
                {
                    var petService = (T)Convert.ChangeType(PetServiceMapper.ToDomainPetService(pet), typeof(T));
                    result.Add(petService);
                });
            }
            else
            {
                throw new ArgumentException("Unsupported type for dropdown");
            }

            return result;
        }
    }
}
