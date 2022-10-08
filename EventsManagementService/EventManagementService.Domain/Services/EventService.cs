using EventManagementService.Domain.Mappers;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public interface IEventService
    {
        Task AddEvent(JobEvent newEvent);
        Task DeleteEventById(int id);
        Task<List<JobEvent>> GetAllJobEvents();
        Task<JobEvent> GetJobEventById(int id);
        Task UpdateJobEvent(JobEvent updateEvent);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Adds new job event.
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task AddEvent(JobEvent newEvent)
        {
            var invalidErrs = newEvent.IsValidEventToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var eventExists = await _eventRepository.JobEventAlreadyExists(0, newEvent.EmployeeId, newEvent.PetId, newEvent.PetServiceId, newEvent.EventDate);
            if (eventExists)
            {
                throw new ArgumentException("This Pet Service for this Pet is already scheduled under this Employee at this date and time.");
            }

            var newEventEntity = EventMapper.FromCoreEvent(newEvent);

            await _eventRepository.AddEvent(newEventEntity);
        }

        /// <summary>
        /// Grabs all events
        /// </summary>
        /// <returns></returns>
        public async Task<List<JobEvent>> GetAllJobEvents() //categories?
        {
            var results = await _eventRepository.GetAllJobEvents();

            var jobEvents = new List<JobEvent>();

            if (results == null || results.Count == 0)
            {
                return jobEvents;
            }

            foreach (var jobEvent in results)
            {
                jobEvents.Add(EventMapper.ToCoreEvent(jobEvent));
            }

            return jobEvents;
        }


        /// <summary>
        /// Gets info for specific event for more info and details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="EntryPointNotFoundException"></exception>
        public async Task<JobEvent> GetJobEventById(int id)
        {
            var jobEvent = await _eventRepository.GetJobEventById(id);

            if (jobEvent == null)
            {
                throw new EntryPointNotFoundException("Event not found.");
            }

            return EventMapper.ToCoreEvent(jobEvent);
        }

        /// <summary>
        /// Update the event
        /// </summary>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdateJobEvent(JobEvent updateEvent)
        {
            var invalidErrs = updateEvent.IsValidEventToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var eventExists = await _eventRepository.JobEventAlreadyExists(updateEvent.Id, updateEvent.EmployeeId, updateEvent.PetId, updateEvent.PetServiceId, updateEvent.EventDate);
            if (eventExists)
            {
                throw new ArgumentException("This Pet Service for this Pet is already scheduled under this Employee at this date and time.");
            }

            var origEvent = GetJobEventById(updateEvent.Id);

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
