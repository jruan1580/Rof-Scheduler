using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class HolidayRateService : IHolidayRateService
    {
        private readonly IHolidayRetrievalRepository _holidayRetrievalRepository;
        private readonly IHolidayAndRatesRepository _holidayAndRatesRepository;
        private readonly IPetServiceRepository _petServiceRepository;

        public HolidayRateService(IPetServiceRepository petServiceRepository,
            IHolidayRetrievalRepository holidayRetrievalRepository,
            IHolidayAndRatesRepository holidayAndRatesRepository)
        {
            _holidayRetrievalRepository = holidayRetrievalRepository;
            _holidayAndRatesRepository = holidayAndRatesRepository;
            _petServiceRepository = petServiceRepository;
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

        public async Task AddHolidayRate(HolidayRate holidayRate)
        {
            await ValidateHolidayRate(holidayRate);

            var holidayRatesEntity = HolidayRatesMapper.FromDomainHolidayRate(holidayRate);

            try
            {
                await _holidayAndRatesRepository.CreateHolidayRates(holidayRatesEntity);
            }
            catch (DbUpdateException ex)
            {
                DbExceptionHandler.HandleDbUpdateException(ex, "Holiday Rate");
            }
        }

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

        public async Task DeleteHolidayRateById(int id)
        {
            await _holidayAndRatesRepository.DeleteHolidayRates(id);
        }

        private async Task ValidateHolidayRate(HolidayRate holidayRate)
        {
            ThrowArgumentExceptionIfHolidayRateOrInnerEntitiesAreNull(holidayRate);

            await ThrowArgumentExceptionIfPetServiceNotFound(holidayRate.PetService.Id);

            await ThrowArgumentExceptionIfHoldayNotFound(holidayRate.Holiday.Id);
        }

        private void ThrowArgumentExceptionIfHolidayRateOrInnerEntitiesAreNull(HolidayRate holidayRate)
        {
            if (holidayRate != null && holidayRate.PetService != null && holidayRate.Holiday != null)
            {
                return;
            }

            throw new ArgumentException("Holiday Rate was not supplied properly.");
        }

        private async Task ThrowArgumentExceptionIfPetServiceNotFound(short id)
        {
            var petServiceEntity = await _petServiceRepository.GetPetServiceById(id);

            if (petServiceEntity != null)
            {
                return;
            }

            throw new ArgumentException($"Pet service with id: {id} was not found.");
        }

        private async Task ThrowArgumentExceptionIfHoldayNotFound(short id)
        {
            var holidayEntity = await _holidayRetrievalRepository.GetHolidayById(id);

            if (holidayEntity != null)
            {
                return;
            }

            throw new ArgumentException($"Holiday with id: {id} was not found.");
        }
    }
}
