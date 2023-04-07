using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using RofShared.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IHolidayRateRetrievalRepository
    {
        Task<HolidayRates> GetHolidayRatesById(int id);
        Task<(List<HolidayRates>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null);
    }

    public class HolidayRateRetrievalRepository : BaseRepository, IHolidayRateRetrievalRepository
    {
        public async Task<(List<HolidayRates>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            using var context = new RofSchedulerContext();

            var holidayRates = await FilterByKeyword(context, keyword?.Trim()?.ToLower());

            var totalPages = DatabaseUtilities.GetTotalPages(holidayRates.Count(), pageSize, page);

            //not more pet services
            if (page > totalPages)
            {
                return (new List<HolidayRates>(), totalPages);
            }

            var skip = (page - 1) * pageSize;
            var result = await SkipNAndTakeTopM(holidayRates, skip, 10);

            GetHolidayAndPetServiceForHolidayRates(context, result);

            return (result, totalPages);
        }

        public async Task<HolidayRates> GetHolidayRatesById(int id)
        {
            return await base.GetEntityById<HolidayRates>(id);
        }

        private async Task<IQueryable<HolidayRates>> FilterByKeyword(RofSchedulerContext context, string keyword)
        {
            var holidayRates = context.HolidayRates.AsQueryable();

            if (string.IsNullOrEmpty(keyword))
            {
                return holidayRates.OrderByDescending(hr => hr.Id);
            }

            //search for all holidays with name that contains keyword
            var holidayIds = await GetHolidaysByKeyword(context, keyword);
            var petServiceIds = await GetPetServiceByKeyword(context, keyword);

            return holidayRates = holidayRates
                .Where(r => holidayIds.Contains(r.HolidayId) ||
                    petServiceIds.Contains(r.PetServiceId))
                .OrderByDescending(hr => hr.Id);
        }

        private async Task<List<short>> GetHolidaysByKeyword(RofSchedulerContext context, string keyword)
        {
            return await context.Holidays
                .Where(h => h.HolidayName.ToLower().Contains(keyword))
                .Select(h => h.Id)
                .ToListAsync();
        }

        private async Task<List<short>> GetPetServiceByKeyword(RofSchedulerContext context, string keyword)
        {
            return await context.PetServices
                        .Where(p => p.ServiceName.ToLower().Contains(keyword))
                        .Select(p => p.Id)
                        .ToListAsync();
        }

        private void GetHolidayAndPetServiceForHolidayRates(RofSchedulerContext context, List<HolidayRates> holidayRates)
        {
            //populate holiday and pet service
            //doing it here because we only need to grab up to pageSize count of petServiceIds and HolidayIds - less load
            var uniquePetServiceIds = holidayRates.Select(r => r.PetServiceId).Distinct().ToList();
            var uniqueHolidayIds = holidayRates.Select(r => r.HolidayId).Distinct().ToList();

            var petServices = context.PetServices.Where(p => uniquePetServiceIds.Contains(p.Id));
            var holidays = context.Holidays.Where(h => uniqueHolidayIds.Contains(h.Id));

            foreach (var holidayRate in holidayRates)
            {
                holidayRate.PetService = petServices.FirstOrDefault(p => p.Id == holidayRate.PetServiceId);
                holidayRate.Holiday = holidays.FirstOrDefault(h => h.Id == holidayRate.HolidayId);
            }
        }
    }
}
