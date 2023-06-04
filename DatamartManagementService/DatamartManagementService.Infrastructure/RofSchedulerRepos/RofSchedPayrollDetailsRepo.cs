using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofSchedulerRepos
{
    public class RofSchedPayrollDetailsRepo
    {
        public async Task<Employee> GetEmployeeById(long id)
        {
            using var context = new RofSchedulerContext();

            return await context.Employee.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<JobEvent>> GetCompletedServicesDoneByEmployee(long id)
        {
            using var context = new RofSchedulerContext();

            var employeeCompletedServices = await context.JobEvent.Where(j => j.EmployeeId == id 
                && j.Completed == true).ToListAsync();

            return employeeCompletedServices;
        }

        public async Task<PetServices> GetPetServiceById(short id)
        {
            using var context = new RofSchedulerContext();

            return await context.PetServices.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<JobEvent> GetJobEventById(int id)
        {
            using var context = new RofSchedulerContext();

            return await context.JobEvent.FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Holidays> CheckIfJobDateIsHoliday(DateTime jobDate)
        {
            using var context = new RofSchedulerContext();

            return await context.Holidays.FirstOrDefaultAsync(h => h.HolidayMonth == jobDate.Month && h.HolidayDay == jobDate.Day);
        }

        public async Task<HolidayRates> GetHolidayRateByHolidayId(short holidayId)
        {
            using var context = new RofSchedulerContext();

            return await context.HolidayRates.FirstOrDefaultAsync(r => r.HolidayId == holidayId);
        }
    }
}
