using EventManagementService.API.DTO;
using EventManagementService.API.DtoMapper;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Mappers;
using EventManagementService.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllJobEventsByMonthAndYear([FromQuery] DateTime date)
        {
            try
            {
                var jobEvents = await _eventService.GetAllJobEventsByMonthAndYear(date);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventById(int id)
        {
            try
            {
                await _eventService.DeleteEventById(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
