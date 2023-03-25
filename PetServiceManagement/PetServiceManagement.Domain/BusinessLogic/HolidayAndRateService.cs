using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IHolidayAndRateService
    {
        Task AddHoliday(Holiday holiday);
        Task AddHolidayRate(HolidayRate holidayRate);
        Task DeleteHolidayById(short id);
        Task DeleteHolidayRateById(int id);
        Task<(List<HolidayRate>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task<(List<Holiday>, int)> GetHolidaysByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task UpdateHoliday(Holiday holiday);
        Task UpdateHolidayRate(HolidayRate holidayRate);
    }

    public class HolidayAndRateService : IHolidayAndRateService
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

            holidays = HolidayMapper.ToHolidayDomains(res.Item1);

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

            try
            {
                await _holidayAndRatesRepository.AddHoliday(holidayEntity);
            }
            catch(Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    throw new ArgumentException("Holiday with same name already exists");
                }

                throw;
            }            
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
            holidayEntity.HolidayMonth = holiday.HolidayMonth;
            holidayEntity.HolidayDay = holiday.HolidayDay;

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

            try
            {
                await _holidayAndRatesRepository.CreateHolidayRates(holidayRatesEntity);
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains(""))
                {
                    throw new ArgumentException("Holiday rate attached to pet service and holiday already exists");
                }

                throw;
            }            
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
                throw new ArgumentException($"Unable to find holiday rate with id: {holidayRate.Id}");
            }

            var holidayRateToUpdate = HolidayRatesMapper.FromDomainHolidayRate(holidayRate);

            await _holidayAndRatesRepository.UpdateHolidayRates(holidayRateToUpdate);
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

            var holidayDate = $"{holiday.HolidayMonth}/{holiday.HolidayDay}/{DateTime.Now.Year}";
            if (!DateTime.TryParse(holidayDate, out var date))
            {
                throw new ArgumentException("Invalid holiday month and day supplied");
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
