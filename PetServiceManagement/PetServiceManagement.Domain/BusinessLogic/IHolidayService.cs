using PetServiceManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IHolidayService
    {
        Task AddHoliday(Holiday holiday);
        Task DeleteHolidayById(short id);
        Task<(List<Holiday>, int)> GetHolidaysByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task UpdateHoliday(Holiday holiday);
    }
}
