using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;
using System;

namespace PetServiceManagement.API.DtoMapper
{
    public static class HolidayDtoMapper
    {
        public static HolidayDTO ToHolidayDTO(Holiday holidayDomain)
        {
            if (holidayDomain == null)
            {
                return null;
            }

            var dto = new HolidayDTO();
            dto.Id = holidayDomain.Id;
            dto.Name = holidayDomain.Name;

            var month = (holidayDomain.HolidayMonth < 10) ? $"0{holidayDomain.HolidayMonth}" : holidayDomain.HolidayMonth.ToString();
            var day = (holidayDomain.HolidayDay < 10) ? $"0{holidayDomain.HolidayDay}" : holidayDomain.HolidayDay.ToString();

            dto.Date = $"{month}/{day}/{DateTime.Now.Year}";

            return dto;
        }

        public static Holiday FromHolidayDTO(HolidayDTO holidayDto)
        {
            if (holidayDto == null)
            {
                return null;
            }

            var domain = new Holiday();
            domain.Id = holidayDto.Id;
            domain.Name = holidayDto.Name;
            if (DateTime.TryParse(holidayDto.Date, out var date))
            {
                domain.HolidayMonth = (short)date.Month;
                domain.HolidayDay = (short)date.Day;
            }

            return domain;
        }
    }
}
