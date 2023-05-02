using EventManagementService.Domain.Mappers.Database;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Persistence;
using RofShared.Exceptions;
using System;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public interface IEventService
    {
        Task AddEvent(JobEvent newEvent);
        Task DeleteEventById(int id);
        Task UpdateJobEvent(JobEvent updateEvent);
    }

    public class EventService : EventBaseService, IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository, 
            IEventRetrievalRepository eventRetrievalRepository) : base(eventRetrievalRepository)
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

            var eventExists = await _eventRetrievalRepository.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(0, newEvent.EmployeeId, newEvent.PetId, newEvent.EventStartTime);
            if (eventExists)
            {
                throw new ArgumentException("This Pet Service for this Pet is already scheduled under this Employee at this date and time.");
            }

            var newEventEntity = EventMapper.FromCoreEvent(newEvent);

            await _eventRepository.AddEvent(newEventEntity);
        }

        public async Task UpdateJobEvent(JobEvent updateEvent)
        {
            var invalidErrs = updateEvent.IsValidEventToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var eventExists = await _eventRetrievalRepository.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(updateEvent.Id, updateEvent.EmployeeId, updateEvent.PetId, updateEvent.EventStartTime);
            if (eventExists)
            {
                throw new ArgumentException("This Pet or Employee is already scheduled for another service at this date and time.");
            }

            var origEvent = await GetDbJobEventById(updateEvent.Id);
            if (origEvent == null)
            {
                throw new EntityNotFoundException("Event was not found. Failed to udpate.");
            }

            var eventEntity = EventMapper.FromCoreEvent(updateEvent);

            await _eventRepository.UpdateJobEvent(eventEntity);
        }

        public async Task DeleteEventById(int id)
        {
            await _eventRepository.DeleteJobEventById(id);
        }
    }
}
