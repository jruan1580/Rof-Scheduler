using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IHolidayAndRatesRepository
    {
        Task<short> AddHoliday(Holidays holiday);
        Task<int> CreateHolidayRates(HolidayRates holidayRates);
        Task DeleteHolidayRates(int id);
        Task<HolidayRates> GetHolidayRatesById(int id);
        Task<(List<HolidayRates>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task RemoveHoliday(short id);
        Task UpdateHoliday(Holidays holiday);
        Task UpdateHolidayRates(HolidayRates holidayRates);
    }

    public class HolidayAndRatesRepository : BaseRepository, IHolidayAndRatesRepository
    {
        /// <summary>
        /// Adds a new holiday date to DB
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        public async Task<short> AddHoliday(Holidays holiday)
        {
            return (await base.CreateEntity(holiday)).Id;
        }

        /// <summary>
        /// Updates a holiday information in DB
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        public async Task UpdateHoliday(Holidays holiday)
        {
            await base.UpdateEntity(holiday);
        }

        /// <summary>
        /// Removes a holiday date from DB.
        /// 
        /// Due to foreign key dependencies, need to delete holiday rate tied to holiday first.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveHoliday(short id)
        {
            using(var context = new RofSchedulerContext())
            {
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

        /// <summary>
        /// Gets holiday rates base on page number and keyword.
        /// Keyword can either be the holiday name or service name.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<(List<HolidayRates>, int)> GetHolidayRatesByPageAndKeyword(int page, int pageSize, string keyword = null)
        {
            using (var context = new RofSchedulerContext())
            {
                var holidayIds = new List<short>();
                var petServiceIds = new List<short>();

                var holidayRates = context.HolidayRates.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    //search for all holidays with name that contains keyword
                    holidayIds = await context.Holidays
                        .Where(h => h.HolidayName.ToLower().Contains(keyword))
                        .Select(h => h.Id)
                        .ToListAsync();

                    //search can also be searching on pet service name
                    petServiceIds = await context.PetServices
                        .Where(p => p.ServiceName.ToLower().Contains(keyword))
                        .Select(p => p.Id)
                        .ToListAsync();

                    //get all rates that has keyword in either holiday naming or service naming
                    holidayRates = holidayRates.Where(r => holidayIds.Contains(r.HolidayId) || petServiceIds.Contains(r.PetServiceId)).AsQueryable();
                }
                
                var totalPages = base.GetTotalPages(holidayRates.Count(), pageSize);

                //not more pet services
                if (page > totalPages)
                {
                    return (new List<HolidayRates>(), totalPages);
                }               

                var skip = (page - 1) * pageSize;
                var result = await holidayRates.OrderByDescending(p => p.Id).Skip(skip).Take(10).ToListAsync();

                //populate holiday and pet service
                //doing it here because we only need to grab up to pageSize count of petServiceIds and HolidayIds - less load
                var uniquePetServiceIds = result.Select(r => r.PetServiceId).Distinct().ToList();
                var uniqueHolidayIds = result.Select(r => r.HolidayId).Distinct().ToList();

                var petServices = context.PetServices.Where(p => uniquePetServiceIds.Contains(p.Id));
                var holidays = context.Holidays.Where(h => uniqueHolidayIds.Contains(h.Id));

                foreach(var holidayRate in result)
                {
                    holidayRate.PetService = petServices.FirstOrDefault(p => p.Id == holidayRate.PetServiceId);
                    holidayRate.Holiday = holidays.FirstOrDefault(h => h.Id == holidayRate.HolidayId);
                }

                return (result, totalPages);
            }
        }

        /// <summary>
        /// Only use to do updates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<HolidayRates> GetHolidayRatesById(int id)
        {
            return await base.GetEntityById<HolidayRates>(id);
        }

        /// <summary>
        /// Adds a rate to a holiday for a particular service
        /// </summary>
        /// <param name="holidayRates"></param>
        /// <returns></returns>
        public async Task<int> CreateHolidayRates(HolidayRates holidayRates)
        {
            return (await base.CreateEntity(holidayRates)).Id;
        }

        /// <summary>
        /// Updates the rate for a holiday to a particular service
        /// </summary>
        /// <param name="holidayRates"></param>
        /// <returns></returns>
        public async Task UpdateHolidayRates(HolidayRates holidayRates)
        {
            using(var context = new RofSchedulerContext())
            {
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
