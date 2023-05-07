using EventManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementService.Infrastructure.Persistence
{
    public interface IEventRetrievalRepository
    {
        Task<bool> DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(int id, long employeeId, long petId, DateTime eventStart);
        Task<List<JobEvent>> GetAllJobEvents();
        Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year);
        Task<JobEvent> GetJobEventById(int id);
    }

    public class EventRetrievalRepository : IEventRetrievalRepository
    {
        public async Task<JobEvent> GetJobEventById(int id)
        {
            using var context = new RofSchedulerContext();

            var result = await context.JobEvents.FirstOrDefaultAsync(j => j.Id == id);

            result.Employee = context.Employees.FirstOrDefault(e => e.Id == result.EmployeeId);
            result.Pet = context.Pets.FirstOrDefault(p => p.Id == result.PetId);
            result.PetService = context.PetServices.FirstOrDefault(s => s.Id == result.PetServiceId);

            return result;
        }

        public async Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year)
        {
            using var context = new RofSchedulerContext();

            IQueryable<JobEvent> allEvents = context.JobEvents;

            var result = await allEvents.Where(j => j.EventStartTime.Month == month && j.EventStartTime.Year == year).ToListAsync();

            await PopulateEmployeePetAndPetService(context, result);

            return result;
        }

        public async Task<List<JobEvent>> GetAllJobEvents()
        {
            using var context = new RofSchedulerContext();

            var result = await context.JobEvents.ToListAsync();

            await PopulateEmployeePetAndPetService(context, result);

            return result;
        }

        public async Task<bool> DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(int id, long employeeId, long petId, DateTime eventStart)
        {
            using var context = new RofSchedulerContext();

            return await context.JobEvents.AnyAsync(j => j.Id != id
                && j.EventStartTime.Equals(eventStart)
                && (j.EmployeeId.Equals(employeeId) || j.PetId.Equals(petId)));
        }

        private async Task PopulateEmployeePetAndPetService(RofSchedulerContext context, List<JobEvent> jobEvents)
        {
            var uniqueEmployeeIds = jobEvents.Select(j => j.EmployeeId).Distinct().ToList();
            var uniquePetIds = jobEvents.Select(j => j.PetId).Distinct().ToList();
            var uniquePetServiceIds = jobEvents.Select(j => j.PetServiceId).Distinct().ToList();

            var employees = await context.Employees.Where(e => uniqueEmployeeIds.Contains(e.Id)).ToListAsync();
            var pets = await context.Pets.Where(p => uniquePetIds.Contains(p.Id)).ToListAsync();
            var petServices = await context.PetServices.Where(ps => uniquePetServiceIds.Contains(ps.Id)).ToListAsync();

            foreach (var jobEvent in jobEvents)
            {
                jobEvent.Employee = employees.FirstOrDefault(e => e.Id == jobEvent.EmployeeId);
                jobEvent.Pet = pets.FirstOrDefault(p => p.Id == jobEvent.PetId);
                jobEvent.PetService = petServices.FirstOrDefault(s => s.Id == jobEvent.PetServiceId);
            }
        }
    }
}
