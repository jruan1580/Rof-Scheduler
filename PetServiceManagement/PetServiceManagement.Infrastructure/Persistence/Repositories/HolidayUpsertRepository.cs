using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IHolidayUpsertRepository
    {
        Task<short> AddHoliday(Holidays holiday);
        Task RemoveHoliday(short id);
        Task UpdateHoliday(Holidays holiday);
    }

    public class HolidayUpsertRepository : BaseRepository, IHolidayUpsertRepository
    {
        public async Task<short> AddHoliday(Holidays holiday)
        {
            return (await base.CreateEntity(holiday)).Id;
        }

        public async Task UpdateHoliday(Holidays holiday)
        {
            await base.UpdateEntity(holiday);
        }

        public async Task RemoveHoliday(short id)
        {
            using var context = new RofSchedulerContext();

            var holidayRates = await context.HolidayRates.Where(r => r.HolidayId == id).ToListAsync();

            if (holidayRates.Count > 0)
            {
                //delete holiday rates tied to holiday
                context.HolidayRates.RemoveRange(holidayRates);
            }

            var holiday = await context.Holidays.FirstOrDefaultAsync(h => h.Id == id);

            if (holiday == null)
            {
                return;
            }

            context.Holidays.Remove(holiday);

            await context.SaveChangesAsync();
        }
    }
}
