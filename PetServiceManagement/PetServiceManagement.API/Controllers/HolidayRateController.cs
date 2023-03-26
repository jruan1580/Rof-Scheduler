﻿using Microsoft.AspNetCore.Authorization;
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
    public class HolidayRateController : ControllerBase
    {
        private readonly IHolidayRateService _holidayRateService;

        public HolidayRateController(IHolidayRateService holidayRateService)
        {
            _holidayRateService = holidayRateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAndKeyword([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var holidayRates = await _holidayRateService.GetHolidayRatesByPageAndKeyword(page, offset, keyword);

            var holidayRateDtos = new List<HolidayRateDTO>();

            holidayRates.Item1.ForEach(hr => holidayRateDtos.Add(HolidayRateDtoMapper.ToHolidayRateDto(hr)));

            return Ok(holidayRateDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddHolidayRate(HolidayRateDTO dto)
        {                     
            var holidayRate = HolidayRateDtoMapper.FromHolidayRateDto(dto);

            await _holidayRateService.AddHolidayRate(holidayRate);

            return Ok();                     
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHolidayRate(HolidayRateDTO dto)
        {
            var holidayRate = HolidayRateDtoMapper.FromHolidayRateDto(dto);

            await _holidayRateService.UpdateHolidayRate(holidayRate);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolidayRate(int id)
        {
            await _holidayRateService.DeleteHolidayRateById(id);

            return Ok();           
        }
    }
}
