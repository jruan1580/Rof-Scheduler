using EventManagementService.API.DTO;
using EventManagementService.API.DtoMapper;
using EventManagementService.API.Filters;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                await _eventService.AddEvent(EventDTOMapper.FromDTOEvent(newEvent));

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobEventsByMonthAndYear([FromQuery] int month, int year)
        {
            try
            {
                var jobEvents = await _eventService.GetAllJobEventsByMonthAndYear(month, year);

                var eventList = new List<EventDTO>();

                foreach (var jobEvent in jobEvents)
                {
                    eventList.Add(EventDTOMapper.ToDTOEvent(jobEvent));
                }

                return Ok(eventList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetJobEventById(int eventId)
        {
            try
            {
                var jobEvent = await _eventService.GetJobEventById(eventId);

                return Ok(EventDTOMapper.ToDTOEvent(jobEvent));
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateJobEvent([FromBody] EventDTO updateEvent)
        {
            try
            {
                var update = EventDTOMapper.FromDTOEvent(updateEvent);

                await _eventService.UpdateJobEvent(update);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEventById(int eventId)
        {
            try
            {
                await _eventService.DeleteEventById(eventId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
