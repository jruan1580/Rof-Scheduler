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
        private readonly IHolidayRateUpsertRepository _holidayRateUpsertRepository;
        private readonly IHolidayRateRetrievalRepository _holidayRateRetrievalRepository;
        private readonly IPetServiceRetrievalRepository _petServiceRetrievalRepository;

        public HolidayRateService(IPetServiceRetrievalRepository petServiceRetrievalRepository,
            IHolidayRetrievalRepository holidayRetrievalRepository,
            IHolidayRateUpsertRepository holidayRateUpsertRepository,
            IHolidayRateRetrievalRepository holidayRateRetrievalRepository)
        {
            _holidayRetrievalRepository = holidayRetrievalRepository;
            _holidayRateUpsertRepository = holidayRateUpsertRepository;
            _holidayRateRetrievalRepository = holidayRateRetrievalRepository;
            _petServiceRetrievalRepository = petServiceRetrievalRepository;
        }

        public async Task<(List<HolidayRate>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            var res = await _holidayRateRetrievalRepository.GetHolidayRatesByPageAndKeyword(page, pageSize, keyword);

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
                await _holidayRateUpsertRepository.CreateHolidayRates(holidayRatesEntity);
            }
            catch (DbUpdateException ex)
            {
                DbExceptionHandler.HandleDbUpdateException(ex, "Holiday Rate");
            }
        }

        public async Task UpdateHolidayRate(HolidayRate holidayRate)
        {
            await ValidateHolidayRate(holidayRate);

            var holidayRateEntity = await _holidayRateRetrievalRepository.GetHolidayRatesById(holidayRate.Id);

            if (holidayRateEntity == null)
            {
                throw new ArgumentException($"Unable to find holiday rate with id: {holidayRate.Id}");
            }

            var holidayRateToUpdate = HolidayRatesMapper.FromDomainHolidayRate(holidayRate);

            await _holidayRateUpsertRepository.UpdateHolidayRates(holidayRateToUpdate);
        }

        public async Task DeleteHolidayRateById(int id)
        {
            await _holidayRateUpsertRepository.DeleteHolidayRates(id);
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
            var petServiceEntity = await _petServiceRetrievalRepository.GetPetServiceById(id);

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
