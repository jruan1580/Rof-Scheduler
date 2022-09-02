using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class HolidayAndRateService
    {
        private readonly IHolidayAndRatesRepository _holidayAndRatesRepository;
        private readonly IPetServiceRepository _petServiceRepository;

        public HolidayAndRateService(IPetServiceRepository petServiceRepository, IHolidayAndRatesRepository holidayAndRatesRepository)
        {
            _holidayAndRatesRepository = holidayAndRatesRepository;
            _petServiceRepository = petServiceRepository;
        }

        /// <summary>
        /// Gets a holiday by page number and searchable holiday name
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<(List<Holiday>, int)> GetHolidaysByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _holidayAndRatesRepository.GetHolidaysByPagesAndSearch(page, pageSize, keyword);

            var holidays = new List<Holiday>();

            if (res.Item1.Count == 0)
            {
                return (holidays, res.Item2);
            }

            res.Item1.ForEach(h => holidays.Add(HolidayMapper.ToHolidayDomain(h)));

            return (holidays, res.Item2);
        }

        /// <summary>
        /// adds a new holiday
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        public async Task AddHoliday(Holiday holiday)
        {
            ValidateHoliday(holiday);

            var holidayEntity = HolidayMapper.FromHolidayDomain(holiday);

            await _holidayAndRatesRepository.AddHoliday(holidayEntity);
        }

        /// <summary>
        /// Updates a holiday.
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdateHoliday(Holiday holiday)
        {
            ValidateHoliday(holiday);

            var holidayEntity = await _holidayAndRatesRepository.GetHolidayById(holiday.Id);

            if (holidayEntity == null)
            {
                throw new ArgumentException($"Holiday with id {holiday.Id} was not found");
            }

            holidayEntity.HolidayName = holiday.Name;
            holidayEntity.HolidayDate = holiday.HolidayDate;

            await _holidayAndRatesRepository.UpdateHoliday(holidayEntity);
        }

        /// <summary>
        /// Deletes all holiday rate tied to holiday first
        /// Removes the holiday after
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteHolidayById(short id)
        {
            //we cannot simply delete holiday because holidayRates has a foreign key to holiday.
            //we need to delete all holiday rate set on holiday first before removing pet services.

            await _holidayAndRatesRepository.RemoveHoliday(id);
        }

        public async Task<(List<HolidayRate>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _holidayAndRatesRepository.GetHolidayRatesByPageAndKeyword(page, pageSize, keyword);

            var holidayRates = new List<HolidayRate>();

            if (res.Item1.Count == 0)
            {
                return (holidayRates, res.Item2);
            }

            res.Item1.ForEach(h => holidayRates.Add(HolidayRatesMapper.ToDomainHolidayRate(h)));

            return (holidayRates, res.Item2);
        }

        /// <summary>
        /// Add holiday rate
        /// </summary>
        /// <param name="holidayRate"></param>
        /// <returns></returns>
        public async Task AddHolidayRate(HolidayRate holidayRate)
        {
            await ValidateHolidayRate(holidayRate);

            var holidayRatesEntity = HolidayRatesMapper.FromDomainHolidayRate(holidayRate);

            await _holidayAndRatesRepository.CreateHolidayRates(holidayRatesEntity);
        }

        /// <summary>
        /// Update holiday rates
        /// </summary>
        /// <param name="holidayRate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdateHolidayRate(HolidayRate holidayRate)
        {
            await ValidateHolidayRate(holidayRate);

            var holidayRateEntity = await _holidayAndRatesRepository.GetHolidayRatesById(holidayRate.Id);

            if (holidayRateEntity == null)
            {
                throw new ArgumentException($"Unable to find holiday rate with id: {holidayRateEntity.Id}");
            }

            holidayRateEntity.PetServiceId = holidayRate.PetService.Id;
            holidayRateEntity.HolidayDateId = holidayRate.Holiday.Id;
            holidayRateEntity.HolidayRate = holidayRate.Rate;

            await _holidayAndRatesRepository.UpdateHolidayRates(holidayRateEntity);
        }

        /// <summary>
        /// Delete holiday rate by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteHolidayRateById(int id)
        {
            await _holidayAndRatesRepository.DeleteHolidayRates(id);
        }

        private void ValidateHoliday(Holiday holiday)
        {
            if (holiday == null)
            {
                throw new ArgumentException("Holiday was not provided");
            }

            if (string.IsNullOrEmpty(holiday.Name))
            {
                throw new ArgumentException("Holiday name was not provided");
            }
        }

        private async Task ValidateHolidayRate(HolidayRate holidayRate)
        {
            if (holidayRate == null || holidayRate.PetService == null || holidayRate.Holiday == null)
            {
                throw new ArgumentException("Holiday Rate was not supplied properly.");
            }

            var petServiceEntity = await _petServiceRepository.GetPetServiceById(holidayRate.PetService.Id);

            if (petServiceEntity == null)
            {
                throw new ArgumentException($"Pet service with id: {holidayRate.PetService.Id} was not found.");
            }

            var holidayEntity = await _holidayAndRatesRepository.GetHolidayById(holidayRate.Holiday.Id);

            if (holidayEntity == null)
            {
                throw new ArgumentException($"Holiday with id: {holidayRate.Holiday.Id} was not found.");
            }
        }
    }
}
