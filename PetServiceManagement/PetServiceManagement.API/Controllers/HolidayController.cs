using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayAndRateService _holidayAndRateService;

        public HolidayController(IHolidayAndRateService holidayAndRateService)
        {
            _holidayAndRateService = holidayAndRateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAndHolidayName([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            try
            {
                var holidays = await _holidayAndRateService.GetHolidaysByPageAndKeyword(page, offset, keyword);

                var holidayDtos = new List<HolidayDTO>();

                holidays.Item1.ForEach(h => holidayDtos.Add(HolidayDtoMapper.ToHolidayDTO(h)));

                return Ok(holidayDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddHoliday([FromBody] HolidayDTO holidayDto)
        {
            try
            {
                var holiday = HolidayDtoMapper.FromHolidayDTO(holidayDto);

                await _holidayAndRateService.AddHoliday(holiday);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHoliday([FromBody] HolidayDTO holidayDto)
        {
            try
            {
                var holiday = HolidayDtoMapper.FromHolidayDTO(holidayDto);

                await _holidayAndRateService.UpdateHoliday(holiday);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteHoliday(short id)
        {
            try
            {
                await _holidayAndRateService.DeleteHolidayById(id);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
