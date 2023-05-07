using EventManagementService.Domain.Mappers.DTO;
using EventManagementService.Domain.Services;
using EventManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.API.Controllers
{
    [CookieActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventUpsertService _eventUpsertService;
        private readonly IEventRetrievalService _eventRetrievalService;

        public EventController(IEventUpsertService eventUpsertService, IEventRetrievalService eventRetrievalService)
        {
            _eventUpsertService = eventUpsertService;
            _eventRetrievalService = eventRetrievalService; 
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventDTO newEvent)
        {
            await _eventUpsertService.AddEvent(EventDTOMapper.FromDTOEvent(newEvent));

            return StatusCode(201);
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobEventsByMonthAndYear([FromQuery] int month, [FromQuery] int year)
        {
            var jobEvents = await _eventRetrievalService.GetAllJobEventsByMonthAndYear(month, year);

            var eventList = new List<EventDTO>();

            foreach (var jobEvent in jobEvents)
            {
                eventList.Add(EventDTOMapper.ToDTOEvent(jobEvent));
            }

            return Ok(eventList);
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllJobEvents()
        {
            var jobEvents = await _eventRetrievalService.GetAllJobEvents();

            var eventList = new List<EventDTO>();

            foreach (var jobEvent in jobEvents)
            {
                eventList.Add(EventDTOMapper.ToDTOEvent(jobEvent));
            }

            return Ok(eventList);
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetJobEventById(int eventId)
        {
            var jobEvent = await _eventRetrievalService.GetJobEventById(eventId);

            return Ok(EventDTOMapper.ToDTOEvent(jobEvent));
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateJobEvent([FromBody] EventDTO updateEvent)
        {
            var update = EventDTOMapper.FromDTOEvent(updateEvent);

            await _eventUpsertService.UpdateJobEvent(update);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEventById(int eventId)
        {
            await _eventUpsertService.DeleteEventById(eventId);

            return Ok();
        }
    }
}
