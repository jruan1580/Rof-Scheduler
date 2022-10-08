using EventManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.Infrastructure
{
    public interface IEventRepository
    {
        Task AddEvent(JobEvent jobEvent);
        Task DeleteJobEvent(int id);
        Task<List<JobEvent>> GetAllJobEvents();
        Task<JobEvent> GetJobEventById(int id);
        Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, short petServiceId, DateTime eventDate);
        Task UpdateJobEvent(JobEvent jobEvent);
    }

    public class EventRepository : IEventRepository
    {
        /// <summary>
        /// Adds new job event.
        /// </summary>
        /// <param name="jobEvent"></param>
        /// <returns></returns>
        public async Task AddEvent(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
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
        /// Displays all job events
        /// </summary>
        /// <returns></returns>
        public async Task<List<JobEvent>> GetAllJobEvents() //update to make by categories (month/year, service, employee, pet)?
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.JobEvents.ToListAsync();
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
        public async Task DeleteJobEvent(int id)
        {
            using (var context = new RofSchedulerContext())
            {
                var job = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);

                if (job != null)
                {
                    throw new ArgumentException($"No job event with id: {id} found.");
                }

                context.Remove(job);

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verify no duplicate event or more than one event for same pet and employee scheduled at one time
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employeeId"></param>
        /// <param name="petId"></param>
        /// <param name="petServiceId"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public async Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, short petServiceId, DateTime eventDate)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.JobEvents.AnyAsync(j => j.Id != id && j.EmployeeId.Equals(employeeId) && j.PetId.Equals(petId)
                    && j.PetServiceId.Equals(petServiceId) && j.EventDate.Equals(eventDate));
            }
        }
    }
}
