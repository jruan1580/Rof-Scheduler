using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class HolidayDropdownService : IDropdownService<Holiday>
    {
        private readonly IHolidayRetrievalRepository _holidayRetrievalRepository;

        public HolidayDropdownService(IHolidayRetrievalRepository holidayRetrievalRepository)
        {
            _holidayRetrievalRepository = holidayRetrievalRepository;
        }

        public async Task<List<Holiday>> GetDropdown()
        {
            var holidays = await _holidayRetrievalRepository.GetAllHolidaysForDropdowns();

            return HolidayMapper.ToHolidayDomains(holidays);
        }
    }
}
