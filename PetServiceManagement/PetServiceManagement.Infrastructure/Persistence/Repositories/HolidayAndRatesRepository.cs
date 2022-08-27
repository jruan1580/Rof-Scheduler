using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public class HolidayAndRatesRepository 
    {
        /// <summary>
        /// gets holidays by search keyword and returns the proper page size.
        /// skip by number of pages already parsed.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<(List<Holidays>, int)> GetHolidaysByPagesAndSearch(int page, int pageSize, string keyword = null)
        {
            using(var context = new RofSchedulerContext())
            {
                IQueryable<Holidays> holidays = null;

                if (string.IsNullOrEmpty(keyword))
                {
                    holidays = context.Holidays.AsQueryable();
                }
                else
                {
                    keyword = keyword.ToLower();
                    holidays = context.Holidays.Where(h => h.HolidayName.ToLower().Contains(keyword));
                }

                var fullCount = holidays.Count();
                var fullPages = fullCount / pageSize; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
                var remaining = fullCount % pageSize; //remaining will be 3 which will be an extra page
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

                //not more pets
                if (page > totalPages)
                {
                    return (new List<Holidays>(), totalPages);
                }

                var skip = (page - 1) * pageSize;
                var result = await holidays.OrderByDescending(p => p.Id).Skip(skip).Take(10).ToListAsync();

                return (result, totalPages);
            }
        }

        /// <summary>
        /// Retrieves a single holiday for updating funcitonality.
        /// Update requires querying for the original record and performing merge.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Holidays> GetHolidayById(short id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Holidays.FirstOrDefaultAsync(h => h.Id == id);
            }
        }

        /// <summary>
        /// Gets a list of all holidays for drop down purposes
        /// </summary>
        /// <returns></returns>
        public async Task<List<Holidays>> GetAllHolidaysForDropdowns()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Holidays.ToListAsync();
            }
        }

        /// <summary>
        /// Adds a new holiday date to DB
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        public async Task<short> AddHoliday(Holidays holiday)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Holidays.Add(holiday);

                await context.SaveChangesAsync();

                return holiday.Id;
            }
        }

        /// <summary>
        /// Updates a holiday information in DB
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        public async Task UpdateHoliday(Holidays holiday)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Holidays.Update(holiday);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes a holiday date from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveHoliday(short id)
        {
            using (var context = new RofSchedulerContext())
            {
                var holiday = await context.Holidays.FirstOrDefaultAsync(h => h.Id == id);

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
                var holidays = new List<short>();
                var petServices = new List<short>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    //search for all holidays with name that contains keyword
                    holidays = await context.Holidays
                        .Where(h => h.HolidayName.ToLower().Contains(keyword))
                        .Select(h => h.Id)
                        .ToListAsync();

                    //search can also be searching on pet service name
                    petServices = await context.PetServices
                        .Where(p => p.ServiceName.ToLower().Contains(keyword))
                        .Select(p => p.Id)
                        .ToListAsync();
                }
            
                //get all rates that has keyword in either holiday naming or service naming
                var holidayRates = context.HolidayRates.Where(r => holidays.Contains(r.HolidayDateId) || petServices.Contains(r.PetServiceId)).AsQueryable();

                var fullCount = holidayRates.Count();
                var fullPages = fullCount / pageSize; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
                var remaining = fullCount % pageSize; //remaining will be 3 which will be an extra page
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

                //not more pet services
                if (page > totalPages)
                {
                    return (new List<HolidayRates>(), totalPages);
                }

                var skip = (page - 1) * pageSize;
                var result = await holidayRates.OrderByDescending(p => p.Id).Skip(skip).Take(10).ToListAsync();

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
            using(var context = new RofSchedulerContext())
            {
                return await context.HolidayRates.FirstOrDefaultAsync(r => r.Id == id);
            }
        }

        /// <summary>
        /// Adds a rate to a holiday for a particular service
        /// </summary>
        /// <param name="holidayRates"></param>
        /// <returns></returns>
        public async Task<int> CreateHolidayRates(HolidayRates holidayRates)
        {
            using(var context = new RofSchedulerContext())
            {
                context.HolidayRates.Add(holidayRates);

                await context.SaveChangesAsync();

                return holidayRates.Id;
            }
        }

        /// <summary>
        /// Updates the rate for a holiday to a particular service
        /// </summary>
        /// <param name="holidayRates"></param>
        /// <returns></returns>
        public async Task UpdateHolidayRates(HolidayRates holidayRates)
        {
            using (var context = new RofSchedulerContext())
            {
                context.HolidayRates.Update(holidayRates);

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
            using (var context = new RofSchedulerContext())
            {
                var holidayRate = await context.HolidayRates.FirstOrDefaultAsync(h => h.Id == id);

                context.HolidayRates.Remove(holidayRate);

                await context.SaveChangesAsync();
            }
        }
    }
}
