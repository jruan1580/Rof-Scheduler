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
            dto.Date = holidayDomain.HolidayDate.ToString("MM/dd/yyyy"); //convert to string of this format

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
                domain.HolidayDate = date;
            }

            return domain;
        }
    }
}
