using PetServiceManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IHolidayRateService
    {
        Task AddHolidayRate(HolidayRate holidayRate);
        Task DeleteHolidayRateById(int id);
        Task<(List<HolidayRate>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task UpdateHolidayRate(HolidayRate holidayRate);
    }
}