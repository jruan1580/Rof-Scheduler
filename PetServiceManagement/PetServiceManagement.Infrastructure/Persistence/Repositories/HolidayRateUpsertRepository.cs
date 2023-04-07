using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IHolidayRateUpsertRepository
    {
        Task<int> CreateHolidayRates(HolidayRates holidayRates);
        Task DeleteHolidayRates(int id);
        Task UpdateHolidayRates(HolidayRates holidayRates);
    }

    public class HolidayRateUpsertRepository : BaseRepository, IHolidayRateUpsertRepository
    {
        public async Task<int> CreateHolidayRates(HolidayRates holidayRates)
        {
            return (await base.CreateEntity(holidayRates)).Id;
        }

        public async Task UpdateHolidayRates(HolidayRates holidayRates)
        {
            using var context = new RofSchedulerContext();

            var holidayRateEntity = await context.HolidayRates.FirstOrDefaultAsync(h => h.Id == holidayRates.Id);

            if (holidayRateEntity == null)
            {
                throw new Exception("Holiday Rate is null");
            }

            holidayRateEntity.PetServiceId = holidayRates.PetServiceId;
            holidayRateEntity.HolidayId = holidayRates.HolidayId;
            holidayRateEntity.HolidayRate = holidayRates.HolidayRate;

            context.Update(holidayRateEntity);

            await context.SaveChangesAsync();

        }

        /// <summary>
        /// Removes the rate for a holiday of a particular service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteHolidayRates(int id)
        {
            await base.DeleteEntity<HolidayRates>(id);
        }
    }
}
