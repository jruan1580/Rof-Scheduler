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
        Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year);
        Task<JobEvent> GetJobEventById(int id);
        Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, DateTime eventStart);
        Task UpdateJobEvent(JobEvent jobEvent);
        Task<List<JobEvent>> GetAllJobEvents();
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

        public async Task<JobEvent> GetJobEventById(int id)
        {
            using (var context = new RofSchedulerContext())
            {
                var result = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);

                result.Employee = context.Employees.FirstOrDefault(e => e.Id == result.EmployeeId);
                result.Pet = context.Pets.FirstOrDefault(p => p.Id == result.PetId);
                result.PetService = context.PetServices.FirstOrDefault(s => s.Id == result.PetServiceId);

                return result;
            }
        }

        public async Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year)
        {
            using (var context = new RofSchedulerContext())
            {
                IQueryable<JobEvent> allEvents = context.JobEvents;

                var result = await allEvents.Where(j => j.EventStartTime.Month == month && j.EventStartTime.Year == year).ToListAsync();

                //populate employee, pet, and pet service
                var uniqueEmployeeIds = result.Select(j => j.EmployeeId).Distinct().ToList();
                var uniquePetIds = result.Select(j => j.PetId).Distinct().ToList();
                var uniquePetServiceIds = result.Select(j => j.PetServiceId).Distinct().ToList();

                var employees = context.Employees.Where(a => uniqueEmployeeIds.Contains(a.Id));
                var pets = context.Pets.Where(a => uniquePetIds.Contains(a.Id));
                var petServices = context.PetServices.Where(a => uniquePetServiceIds.Contains(a.Id));

                foreach(var jobEvent in result)
                {
                    jobEvent.Employee = employees.FirstOrDefault(e => e.Id == jobEvent.EmployeeId);
                    jobEvent.Pet = pets.FirstOrDefault(p => p.Id == jobEvent.PetId);
                    jobEvent.PetService = petServices.FirstOrDefault(s => s.Id == jobEvent.PetServiceId);
                }

                return result;
            }
        }

        public async Task<List<JobEvent>> GetAllJobEvents()
        {
            using (var context = new RofSchedulerContext())
            {
                var result = await context.JobEvents.ToListAsync();

                //populate employee, pet, and pet service
                var uniqueEmployeeIds = result.Select(j => j.EmployeeId).Distinct().ToList();
                var uniquePetIds = result.Select(j => j.PetId).Distinct().ToList();
                var uniquePetServiceIds = result.Select(j => j.PetServiceId).Distinct().ToList();

                var employees = context.Employees.Where(a => uniqueEmployeeIds.Contains(a.Id));
                var pets = context.Pets.Where(a => uniquePetIds.Contains(a.Id));
                var petServices = context.PetServices.Where(a => uniquePetServiceIds.Contains(a.Id));

                foreach (var jobEvent in result)
                {
                    jobEvent.Employee = employees.FirstOrDefault(e => e.Id == jobEvent.EmployeeId);
                    jobEvent.Pet = pets.FirstOrDefault(p => p.Id == jobEvent.PetId);
                    jobEvent.PetService = petServices.FirstOrDefault(s => s.Id == jobEvent.PetServiceId);
                }

                return result;
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

        public async Task<bool> JobEventAlreadyExists(int id, long employeeId, long petId, DateTime eventStart)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.JobEvents.AnyAsync(j => j.Id != id && j.EventStartTime.Equals(eventStart) && (j.EmployeeId.Equals(employeeId) || j.PetId.Equals(petId)));
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
