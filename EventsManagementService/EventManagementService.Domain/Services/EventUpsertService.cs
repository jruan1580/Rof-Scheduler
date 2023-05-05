using EventManagementService.Domain.Mappers.Database;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using DbEvent = EventManagementService.Infrastructure.Persistence.Entities.JobEvent;

namespace EventManagementService.Domain.Services
{
    public class EventUpsertService : EventService, IEventUpsertService
    {
        private readonly IEventUpsertRepository _eventUpsertRepository;

        public EventUpsertService(IEventUpsertRepository eventRepository,
            IEventRetrievalRepository eventRetrievalRepository) : base(eventRetrievalRepository)
        {
            _eventUpsertRepository = eventRepository;
        }

        public async Task AddEvent(JobEvent newEvent)
        {
            await ValidateEvent(newEvent, false);

            var newEventEntity = EventMapper.FromCoreEvent(newEvent);

            await _eventUpsertRepository.AddEvent(newEventEntity);
        }

        public async Task UpdateJobEvent(JobEvent updateEvent)
        {
            await ValidateEvent(updateEvent, true);

            var origEvent = await GetDbJobEventById(updateEvent.Id);

            MergeEventPropertiesForUpdate(origEvent, updateEvent);

            await _eventUpsertRepository.UpdateJobEvent(origEvent);
        }

        public async Task DeleteEventById(int id)
        {
            await _eventUpsertRepository.DeleteJobEventById(id);
        }

        private async Task ValidateEvent(JobEvent jobEvent, bool isUpdate)
        {
            ValidateEventProperties(jobEvent, isUpdate);

            await ValidateIfThereIsEventOverlapForSamePetOrEmployee(jobEvent.Id,
                jobEvent.EmployeeId,
                jobEvent.PetId,
                jobEvent.EventStartTime);
        }

        private void ValidateEventProperties(JobEvent jobEvent, bool isUpdate)
        {
            var validationErrors = (isUpdate) ? jobEvent.GetValidationErrorsForUpdate() : jobEvent.GetValidationErrorsForBothCreateOrUpdate();

            if (validationErrors.Count > 0)
            {
                var errorMessage = string.Join("\n", validationErrors);

                throw new ArgumentException(errorMessage);
            }
        }

        private async Task ValidateIfThereIsEventOverlapForSamePetOrEmployee(int id, long employeeId, long petId, DateTime eventStartTime)
        {
            var hasOverlap = await _eventRetrievalRepository.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(id, employeeId, petId, eventStartTime);

            if (hasOverlap)
            {
                throw new ArgumentException("Employee or Pet already scheduled for a service at this time.");
            }
        }

        private void MergeEventPropertiesForUpdate(DbEvent originalEvent, JobEvent updateEvent)
        {
            originalEvent.EmployeeId = updateEvent.EmployeeId;
            originalEvent.PetId = updateEvent.PetId;
            originalEvent.PetServiceId = updateEvent.PetServiceId;
            originalEvent.EventStartTime = updateEvent.EventStartTime;
        }
    }
}
