using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofSchedulerRepos
{
    //public long EmployeeId { get; set; }
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
    //public long PetServiceId { get; set; }
    //public string PetServiceName { get; set; }
    //public decimal EmployeePayForService { get; set; }
    //public bool IsHolidayPay { get; set; }
    //public int ServiceDuration { get; set; }
    //public string ServiceDurationTimeUnit { get; set; }
    //public long JobEventId { get; set; }
    //public DateTime ServiceStartDateTime { get; set; }
    //public DateTime ServiceEndDateTime { get; set; }

    public class RofSchedPayrollDetailsRepo
    {
        //public async Task<List<Employee>> GetEmployeeDetails()
        //{
        //    using var context = new RofSchedulerContext();

        //    return await context.Employee.Select(e => new Employee() { Id = e.Id, FirstName = e.FirstName, LastName = e.LastName }).ToListAsync();
        //}

        //public async Task<List<PetServices>> GetPetServiceDetails()
        //{
        //    using var context = new RofSchedulerContext();

        //    return await context.PetServices.Select(s => new PetServices() { Id = s.Id, ServiceName = s.ServiceName, Duration = s.Duration, TimeUnit = s.TimeUnit }).ToListAsync();
        //}

        public async Task<List<JobEvent>> GetCompletedJobEventsOfAnEmployeeForCertainPeriod(long id, DateTime startTime, DateTime endPe)
        {
            using var context = new RofSchedulerContext();

            var employeeCompletedJobEvents = await context.JobEvent.Where(j => 
                j.EmployeeId == id 
                && j.Completed == true 
                && (j.EventStartTime.Month >= startTime.Month && j.EventEndTime).ToListAsync();
        }

        private bool CheckIfEventWasOnHoliday(JobEvent jobEvent)
        {
            using var context = new RofSchedulerContext();

            var holiday = context.Holidays.FirstOrDefaultAsync(h => h.HolidayMonth == jobEvent.EventStartTime.Month && h.HolidayDay == jobEvent.EventStartTime.Day);

            return holiday != null;
        }
    }
}
