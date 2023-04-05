using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class HolidayService : IHolidayService
    {
        private IHolidayAndRatesRepository _holidayAndRatesRepository;

        private IHolidayRetrievalRepository _holidayRetrievalRepository;

        public HolidayService(IHolidayAndRatesRepository holidayAndRatesRepository, 
            IHolidayRetrievalRepository holidayRetrievalRepository)
        {
            _holidayAndRatesRepository = holidayAndRatesRepository;

            _holidayRetrievalRepository = holidayRetrievalRepository;
        }

        public async Task<(List<Holiday>, int)> GetHolidaysByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _holidayRetrievalRepository.GetHolidaysByPagesAndSearch(page, pageSize, keyword);

            if (res.Item1.Count == 0)
            {
                return (new List<Holiday>(), res.Item2);
            }

            var holidays = HolidayMapper.ToHolidayDomains(res.Item1);

            return (holidays, res.Item2);
        }

        public async Task AddHoliday(Holiday holiday)
        {
            ThrowArgumentExceptionIfValidationFails(holiday);

            var holidayEntity = HolidayMapper.FromHolidayDomain(holiday);

            try
            {
                await _holidayAndRatesRepository.AddHoliday(holidayEntity);
            }
            catch (DbUpdateException e)
            {
                DbExceptionHandler.HandleDbUpdateException(e, "Holiday");
            }
        }

        public async Task UpdateHoliday(Holiday holiday)
        {
            ThrowArgumentExceptionIfValidationFails(holiday);

            var holidayEntity = await _holidayRetrievalRepository.GetHolidayById(holiday.Id);

            if (holidayEntity == null)
            {
                throw new ArgumentException($"Holiday with id {holiday.Id} was not found");
            }

            holidayEntity.HolidayName = holiday.Name;
            holidayEntity.HolidayMonth = holiday.HolidayMonth;
            holidayEntity.HolidayDay = holiday.HolidayDay;

            await _holidayAndRatesRepository.UpdateHoliday(holidayEntity);
        }

        public async Task DeleteHolidayById(short id)
        {
            await _holidayAndRatesRepository.RemoveHoliday(id);
        }

        private void ThrowArgumentExceptionIfValidationFails(Holiday holiday)
        {
            if (holiday == null)
            {
                throw new ArgumentException("Holiday was not provided");
            }

            var validationErrors = holiday.GetHolidayValidationErrors();

            if (string.IsNullOrEmpty(validationErrors))
            {
                return;
            }

            throw new ArgumentException(validationErrors);
        }
    }
}
