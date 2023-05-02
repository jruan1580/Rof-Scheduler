using EventManagementService.Domain.Mappers.Database;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public class EventRetrievalService : EventBaseService, IEventRetrievalService
    {
        public EventRetrievalService(IEventRetrievalRepository eventRetrievalRepository)
            : base(eventRetrievalRepository) { }

        public async Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year)
        {
            var results = await _eventRetrievalRepository.GetAllJobEventsByMonthAndYear(month, year);

            return new List<JobEvent>(results.Select(e => EventMapper.ToCoreEvent(e))).ToList();
        }

        public async Task<List<JobEvent>> GetAllJobEvents()
        {
            var results = await _eventRetrievalRepository.GetAllJobEvents();

            return new List<JobEvent>(results.Select(e => EventMapper.ToCoreEvent(e))).ToList();
        }

        public async Task<JobEvent> GetJobEventById(int id)
        {
            var jobEvent = await GetDbJobEventById(id);

            return EventMapper.ToCoreEvent(jobEvent);
        }
    }
}
