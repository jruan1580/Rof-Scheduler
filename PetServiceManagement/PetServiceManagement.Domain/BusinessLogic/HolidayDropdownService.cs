using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class HolidayDropdownService : IDropdownService<Holiday>
    {
        private readonly IHolidayAndRatesRepository _holidayAndRatesRepository;

        public HolidayDropdownService(IHolidayAndRatesRepository holidayAndRatesRepository)
        {
            _holidayAndRatesRepository = holidayAndRatesRepository;
        }

        public async Task<List<Holiday>> GetDropdown()
        {
            var holidays = await _holidayAndRatesRepository.GetAllHolidaysForDropdowns();

            return HolidayMapper.ToHolidayDomains(holidays);
        }
    }
}
