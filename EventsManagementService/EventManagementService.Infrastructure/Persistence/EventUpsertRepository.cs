using EventManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace EventManagementService.Infrastructure.Persistence
{
    public interface IEventUpsertRepository
    {
        Task AddEvent(JobEvent jobEvent);
        Task DeleteJobEventById(int id);
        Task UpdateJobEvent(JobEvent jobEvent);
    }

    public class EventUpsertRepository : IEventUpsertRepository
    {
        public async Task AddEvent(JobEvent jobEvent)
        {
            using var context = new RofSchedulerContext();

            await CalculateEndTime(jobEvent);

            context.JobEvents.Add(jobEvent);

            await context.SaveChangesAsync();
        }

        public async Task UpdateJobEvent(JobEvent updateEvent)
        {
            using var context = new RofSchedulerContext();

            await CalculateEndTime(updateEvent);

            context.Update(updateEvent);

            await context.SaveChangesAsync();
        }

        public async Task DeleteJobEventById(int id)
        {
            using var context = new RofSchedulerContext();

            var job = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                throw new EntityNotFoundException("Event");
            }

            context.Remove(job);

            await context.SaveChangesAsync();
        }

        private async Task CalculateEndTime(JobEvent jobEvent)
        {
            using var context = new RofSchedulerContext();

            var petService = await context.PetServices.FirstOrDefaultAsync(ps => ps.Id == jobEvent.PetServiceId);

            if (petService.TimeUnit.ToLower() == "hours")
            {
                jobEvent.EventEndTime = jobEvent.EventStartTime.AddHours(petService.Duration);
            }

            if (petService.TimeUnit.ToLower() == "minutes")
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
