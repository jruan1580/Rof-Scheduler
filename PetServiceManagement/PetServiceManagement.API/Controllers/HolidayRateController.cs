using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayRateController : ControllerBase
    {
        private readonly IHolidayAndRateService _holidayAndRateService;

        public HolidayRateController(IHolidayAndRateService holidayAndRateService)
        {
            _holidayAndRateService = holidayAndRateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAndKeyword([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            try
            {
                var holidayRates = await _holidayAndRateService.GetHolidayRatesByPageAndKeyword(page, offset, keyword);

                var holidayRateDtos = new List<HolidayRateDTO>();

                holidayRates.Item1.ForEach(hr => holidayRateDtos.Add(HolidayRateDtoMapper.ToHolidayRateDto(hr)));

                return Ok(holidayRateDtos);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddHolidayRate(HolidayRateDTO dto)
        {
            try
            {
                var holidayRate = HolidayRateDtoMapper.FromHolidayRateDto(dto);

                await _holidayAndRateService.AddHolidayRate(holidayRate);

                return Ok();
            }
            catch(ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch(Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Violation of UNIQUE KEY constraint"))
                {
                    return BadRequest("Holiday rate attached to pet service and holiday already exists");
                }
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHolidayRate(HolidayRateDTO dto)
        {
            try
            {
                var holidayRate = HolidayRateDtoMapper.FromHolidayRateDto(dto);

                await _holidayAndRateService.UpdateHolidayRate(holidayRate);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolidayRate(int id)
        {
            try
            {
                await _holidayAndRateService.DeleteHolidayRateById(id);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
