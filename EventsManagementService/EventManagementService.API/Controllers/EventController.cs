using EventManagementService.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
