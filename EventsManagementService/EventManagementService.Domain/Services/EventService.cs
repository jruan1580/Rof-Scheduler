using EventManagementService.Infrastructure.Persistence;
using RofShared.Exceptions;
using System.Threading.Tasks;
using EventDb = EventManagementService.Infrastructure.Persistence.Entities.JobEvent;

namespace EventManagementService.Domain.Services
{
    public class EventService
    {
        protected readonly IEventRetrievalRepository _eventRetrievalRepository;

        public EventService(IEventRetrievalRepository eventRetrievalRepository)
        {
            _eventRetrievalRepository = eventRetrievalRepository;
        }

        protected async Task<EventDb> GetDbJobEventById(int id)
        {
            var jobEvent = await _eventRetrievalRepository.GetJobEventById(id);

            if(jobEvent == null)
            {
                throw new EntityNotFoundException("Event");
            }

            return jobEvent;
        }
    }
}
