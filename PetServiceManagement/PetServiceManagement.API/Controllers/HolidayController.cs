using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using RofShared.FilterAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAndHolidayName([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var holidays = await _holidayService.GetHolidaysByPageAndKeyword(page, offset, keyword);

            var holidayDtos = new List<HolidayDTO>();

            holidays.Item1.ForEach(h => holidayDtos.Add(HolidayDtoMapper.ToHolidayDTO(h)));

            return Ok(new HolidayWithTotalPagesDto(holidayDtos, holidays.Item2));
        }

        [HttpPost]
        public async Task<IActionResult> AddHoliday([FromBody] HolidayDTO holidayDto)
        {
            var holiday = HolidayDtoMapper.FromHolidayDTO(holidayDto);

            await _holidayService.AddHoliday(holiday);

            return Ok();           
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHoliday([FromBody] HolidayDTO holidayDto)
        {
            var holiday = HolidayDtoMapper.FromHolidayDTO(holidayDto);

            await _holidayService.UpdateHoliday(holiday);

            return Ok();           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(short id)
        {
            await _holidayService.DeleteHolidayById(id);

            return Ok();           
        }
    }
}
