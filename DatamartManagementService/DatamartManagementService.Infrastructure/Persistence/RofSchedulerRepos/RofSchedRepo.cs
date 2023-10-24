using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos
{
    public class RofSchedRepo : IRofSchedRepo
    {
        public async Task<Employee> GetEmployeeById(long id)
        {
            using var context = new RofSchedulerContext();

            return await context.Employee.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<JobEvent>> GetCompletedServicesBetweenDates(DateTime startDate,
            DateTime endDate)
        {
            using var context = new RofSchedulerContext();

            return await context.JobEvent
                .Where(j => j.EventStartTime > startDate
                    && j.EventEndTime <= endDate
                    && j.Completed == true)
                .OrderBy(j => j.EmployeeId)
                .ThenBy(j => j.EventStartTime)
                .ToListAsync();
        }

        public async Task<List<JobEvent>> GetCompletedServicesUpUntilDate(DateTime date)
        {
            using var context = new RofSchedulerContext();

            return await context.JobEvent
                .Where(j => j.EventEndTime <= date
                    && j.Completed == true)
                .OrderBy(j => j.EmployeeId)
                .ThenBy(j => j.EventStartTime)
                .ToListAsync();
        }

        public async Task<PetServices> GetPetServiceById(short id)
        {
            using var context = new RofSchedulerContext();

            return await context.PetServices.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Holidays> CheckIfJobDateIsHoliday(DateTime jobDate)
        {
            using var context = new RofSchedulerContext();

            return await context.Holidays.FirstOrDefaultAsync(h =>
                h.HolidayMonth == jobDate.Month &&
                h.HolidayDay == jobDate.Day);
        }

        public async Task<HolidayRates> GetHolidayRateByPetServiceId(short petServiceId)
        {
            using var context = new RofSchedulerContext();

            return await context.HolidayRates
                .FirstOrDefaultAsync(r => r.PetServiceId == petServiceId);
        }
    }
}
