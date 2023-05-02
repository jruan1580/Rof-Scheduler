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
        Task UpdateJobEvent(JobEvent jobEvent);
    }

    public class EventRepository : IEventRepository
    {
        public async Task AddEvent(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
                await CalculateEndTime(jobEvent);

                context.JobEvents.Add(jobEvent);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateJobEvent(JobEvent jobEvent)
        {
            using (var context = new RofSchedulerContext())
            {
                var origEvent = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == jobEvent.Id);
                
                await CalculateEndTime(jobEvent);
                
                origEvent.EmployeeId = jobEvent.EmployeeId;
                origEvent.PetId = jobEvent.PetId;
                origEvent.PetServiceId = jobEvent.PetServiceId;

                origEvent.EventStartTime = jobEvent.EventStartTime;
                origEvent.EventEndTime = jobEvent.EventEndTime;
                origEvent.Completed = jobEvent.Completed;

                context.Update(origEvent);

                await context.SaveChangesAsync();
            }
        }

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
