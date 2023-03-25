using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
using RofShared.FilterAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdownService<PetService> _petServiceDropdown;
        private readonly IDropdownService<Holiday> _holidayDropdown;

        public DropdownController(IDropdownService<PetService> petDropdownService,
            IDropdownService<Holiday> holidayDropdownService)
        {
            _holidayDropdown = holidayDropdownService;

            _petServiceDropdown = petDropdownService;
        }

        [HttpGet("petServices")]
        public async Task<IActionResult> GetPetServices()
        {
            var petServices = await _petServiceDropdown.GetDropdown();

            var petServiceDtos = new List<PetServiceDTO>();

            petServices.ForEach(service => petServiceDtos.Add(PetServiceDtoMapper.ToPetServiceDTO(service)));

            return Ok(petServiceDtos);
        }

        [HttpGet("holidays")]
        public async Task<IActionResult> GetHolidays()
        {
            var holidays = await _holidayDropdown.GetDropdown();

            var holidayDtos = new List<HolidayDTO>();

            holidays.ForEach(h => holidayDtos.Add(HolidayDtoMapper.ToHolidayDTO(h)));

            return Ok(holidayDtos);
        }
    }
}
