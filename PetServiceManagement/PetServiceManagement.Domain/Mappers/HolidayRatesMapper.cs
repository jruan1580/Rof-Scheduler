using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Domain.Mappers
{
    public class HolidayRatesMapper
    {
        public HolidayRate ToDomainHolidayRate(HolidayRates holidayRates)
        {
            if (holidayRates == null)
            {
                return null;
            }

            var holidayRate = new HolidayRate();
            
            holidayRate.Rate = holidayRates.HolidayRate;
            
            if (holidayRates.PetService != null)
            {
                holidayRate.PetService = PetServiceMapper.ToDomainPetService(holidayRates.PetService);
            }

            if (holidayRates.HolidayDate != null)
            {
                holidayRate.Holiday = HolidayMapper.ToHolidayDomain(holidayRates.HolidayDate);
            }

            return holidayRate;
        }

        public HolidayRates FromDomainHolidayRate(HolidayRate holidayRate)
        {
            if (holidayRate == null)
            {
                return null;
            }

            var holidayRates = new HolidayRates();

            holidayRates.Id = holidayRate.Id;
            holidayRates.HolidayRate = holidayRate.Rate;

            //really only need the ids - don't map more than necessary
            if (holidayRate.PetService != null)
            {
                holidayRates.PetServiceId = holidayRate.PetService.Id;                
            }

            //really only need the ids - don't map more than necessary
            if (holidayRate.Holiday != null)
            {
                holidayRates.HolidayDateId = holidayRate.Holiday.Id;
            }

            return holidayRates;
        }
    }
}
