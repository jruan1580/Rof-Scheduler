using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using RofShared.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IHolidayRetrievalRepository
    {
        Task<List<Holidays>> GetAllHolidaysForDropdowns();
        Task<Holidays> GetHolidayById(short id);
        Task<(List<Holidays>, int)> GetHolidaysByPagesAndSearch(int page, int pageSize, string keyword = null);
    }

    public class HolidayRetrievalRepository : BaseRepository, IHolidayRetrievalRepository
    {
        public async Task<(List<Holidays>, int)> GetHolidaysByPagesAndSearch(int page, int pageSize, string keyword = null)
        {
            using var context = new RofSchedulerContext();

            IQueryable<Holidays> holidays = context.Holidays;

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                holidays = context.Holidays.Where(h =>
                    h.HolidayName.ToLower().Contains(keyword));
            }

            var totalPages = DatabaseUtilities.GetTotalPages(holidays.Count(), pageSize, page);

            //not more pets
            if (page > totalPages)
            {
                return (new List<Holidays>(), totalPages);
            }

            var skip = (page - 1) * pageSize;
            var result = await holidays.OrderByDescending(p => p.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return (result, totalPages);
        }

        public async Task<Holidays> GetHolidayById(short id)
        {
            using var context = new RofSchedulerContext();

            return await base.GetEntityById<Holidays>(id);
        }

        public async Task<List<Holidays>> GetAllHolidaysForDropdowns()
        {
            using var context = new RofSchedulerContext();

            return await context.Holidays.ToListAsync();
        }
    }
}
