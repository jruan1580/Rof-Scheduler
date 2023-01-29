using EventManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementService.Infrastructure.Persistence
{
    public interface IEventRepository
    {
        Task AddEvent(JobEvent jobEvent);
        Task DeleteJobEventById(int id);
        Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(DateTime eventDate);
        Task<JobEvent> GetJobEventById(int id);
        Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, DateTime eventStart);
        Task UpdateJobEvent(JobEvent jobEvent);
    }

    public class EventRepository : IEventRepository
    {
        /// <summary>
        /// Calculate and update the end time.
        /// Adds new job event.
        /// </summary>
        /// <param name="jobEvent"></param>
        /// <returns></returns>
        public async Task AddEvent(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
                await CalculateEndTime(jobEvent);

                context.JobEvents.Add(jobEvent);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Use to get all details of an individual job event
        /// Or used to get original version of the event for updates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JobEvent> GetJobEventById(int id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);
            }
        }

        /// <summary>
        /// Displays all job events for specific month & year
        /// </summary>
        /// <returns></returns>
        public async Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(DateTime eventDate)
        {
            using (var context = new RofSchedulerContext())
            {
                IQueryable<JobEvent> allEvents = context.JobEvents;

                var result = await allEvents.Where(e => e.EventStartTime.Month == eventDate.Month && e.EventStartTime.Year == eventDate.Year).ToListAsync();

                return result;
            }
        }

        /// <summary>
        /// Updates a job service
        /// </summary>
        /// <returns></returns>
        public async Task UpdateJobEvent(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
                await CalculateEndTime(jobEvent);

                context.Update(jobEvent);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes an event
        /// Maybe also used to remove upon completion of job?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task DeleteJobEventById(int id)
        {
            using (var context = new RofSchedulerContext())
            {
                var job = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);

                if (job == null)
                {
                    throw new ArgumentException($"No job event with id: {id} found.");
                }

                context.Remove(job);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verify no same pet and employee scheduled at one time
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employeeId"></param>
        /// <param name="petId"></param>
        /// <param name="petServiceId"></param>
        /// <param name="eventStart"></param>
        /// <returns></returns>
        public async Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, DateTime eventStart)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.JobEvents.AnyAsync(j => j.Id != id && j.EventStartTime.Equals(eventStart) && (j.EmployeeId.Equals(employeeId) || j.PetId.Equals(petId)));
            }
        }

        /// <summary>
        /// Grabs duration and unit of pet service & adds duration to start time to get end time
        /// </summary>
        /// <param name="jobEvent"></param>
        /// <returns></returns>
        private async Task CalculateEndTime(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
                var petService = await context.PetServices.FirstOrDefaultAsync(ps => ps.Id == jobEvent.PetServiceId);

                if(petService.TimeUnit.ToLower() == "hours")
                {
                    jobEvent.EventEndTime = jobEvent.EventStartTime.AddHours(petService.Duration);
                }

                if(petService.TimeUnit.ToLower() == "minutes")
                {
                    jobEvent.EventEndTime = jobEvent.EventStartTime.AddMinutes(petService.Duration);
                }

                if (petService.TimeUnit.ToLower() == "seconds")
                {
                    jobEvent.EventEndTime = jobEvent.EventStartTime.AddSeconds(petService.Duration);
                }
            }
        }
    }
}
