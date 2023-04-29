using EventManagementService.API.DTO;
using EventManagementService.API.DtoMapper;
using EventManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
using RofShared.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.API.Controllers
{
    [CookieActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventDTO newEvent)
        {
            await _eventService.AddEvent(EventDTOMapper.FromDTOEvent(newEvent));

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobEventsByMonthAndYear([FromQuery] int month, [FromQuery] int year)
        {
            var jobEvents = await _eventService.GetAllJobEventsByMonthAndYear(month, year);

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
            var jobEvents = await _eventService.GetAllJobEvents();

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
            var jobEvent = await _eventService.GetJobEventById(eventId);

            return Ok(EventDTOMapper.ToDTOEvent(jobEvent));
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateJobEvent([FromBody] EventDTO updateEvent)
        {
            var update = EventDTOMapper.FromDTOEvent(updateEvent);

            await _eventService.UpdateJobEvent(update);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEventById(int eventId)
        {
            await _eventService.DeleteEventById(eventId);

            return Ok();
        }
    }
}
