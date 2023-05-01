using EventManagementService.Domain.Mappers.Database;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Persistence;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public interface IEventService
    {
        Task AddEvent(JobEvent newEvent);
        Task DeleteEventById(int id);
        Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year);
        Task<JobEvent> GetJobEventById(int id);
        Task UpdateJobEvent(JobEvent updateEvent);
        Task<List<JobEvent>> GetAllJobEvents();
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task AddEvent(JobEvent newEvent)
        {
            var invalidErrs = newEvent.IsValidEventToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var eventExists = await _eventRepository.JobEventAlreadyExists(0, newEvent.EmployeeId, newEvent.PetId, newEvent.EventStartTime);
            if (eventExists)
            {
                throw new ArgumentException("This Pet Service for this Pet is already scheduled under this Employee at this date and time.");
            }

            var newEventEntity = EventMapper.FromCoreEvent(newEvent);

            await _eventRepository.AddEvent(newEventEntity);
        }

        public async Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year)
        {
            var results = await _eventRepository.GetAllJobEventsByMonthAndYear(month, year);

            if (results == null || results.Count == 0)
            {
                return new List<JobEvent>();
            }

            return new List<JobEvent>(results.Select(e => EventMapper.ToCoreEvent(e))).ToList();
        }

        public async Task<List<JobEvent>> GetAllJobEvents()
        {
            var results = await _eventRepository.GetAllJobEvents();

            if (results == null || results.Count == 0)
            {
                return new List<JobEvent>();
            }

            return new List<JobEvent>(results.Select(e => EventMapper.ToCoreEvent(e))).ToList();
        }
        
        public async Task<JobEvent> GetJobEventById(int id)
        {
            var jobEvent = await _eventRepository.GetJobEventById(id);

            if (jobEvent == null)
            {
                throw new EntityNotFoundException("Event not found.");
            }

            return EventMapper.ToCoreEvent(jobEvent);
        }

        public async Task UpdateJobEvent(JobEvent updateEvent)
        {
            var invalidErrs = updateEvent.IsValidEventToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var eventExists = await _eventRepository.JobEventAlreadyExists(updateEvent.Id, updateEvent.EmployeeId, updateEvent.PetId, updateEvent.EventStartTime);
            if (eventExists)
            {
                throw new ArgumentException("This Pet or Employee is already scheduled for another service at this date and time.");
            }

            var origEvent = GetJobEventById(updateEvent.Id);
            if (origEvent == null)
            {
                throw new EntityNotFoundException("Event was not found. Failed to udpate.");
            }

            var eventEntity = EventMapper.FromCoreEvent(updateEvent);

            await _eventRepository.UpdateJobEvent(eventEntity);
        }

        /// <summary>
        /// Removes event.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteEventById(int id)
        {
            await _eventRepository.DeleteJobEventById(id);
        }
    }
}
