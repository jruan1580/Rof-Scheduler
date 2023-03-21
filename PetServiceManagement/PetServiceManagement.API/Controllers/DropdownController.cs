using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
using RofShared.FilterAttributes;
using System;
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
        private readonly IDropDownService _dropDownService;

        public DropdownController(IDropDownService dropdownService)
        {
            _dropDownService = dropdownService;
        }

        [HttpGet("petServices")]
        public async Task<IActionResult> GetPetServices()
        {
            try
            {
                var petServices = await _dropDownService.GetDropdownForType<PetService>();

                var petServiceDtos = new List<PetServiceDTO>();

                petServices.ForEach(service => petServiceDtos.Add(PetServiceDtoMapper.ToPetServiceDTO(service)));

                return Ok(petServiceDtos);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("holidays")]
        public async Task<IActionResult> GetHolidays()
        {
            try
            {
                var holidays = await _dropDownService.GetDropdownForType<Holiday>();

                var holidayDtos = new List<HolidayDTO>();

                holidays.ForEach(h => holidayDtos.Add(HolidayDtoMapper.ToHolidayDTO(h)));

                return Ok(holidayDtos);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
