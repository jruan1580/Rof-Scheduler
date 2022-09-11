using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;

namespace PetServiceManagement.API.DtoMapper
{
    public static class HolidayRateDtoMapper
    {
        public static HolidayRateDTO ToHolidayRateDto(HolidayRate holidayRate)
        {
            if (holidayRate == null)
            {
                return null;
            }

            var holidayRateDto = new HolidayRateDTO();
            holidayRateDto.Id = holidayRate.Id;
            holidayRateDto.Rate = holidayRate.Rate;

            if (holidayRate.PetService != null)
            {
                holidayRateDto.PetService = PetServiceDtoMapper.ToPetServiceDTO(holidayRate.PetService);
            }

            if (holidayRate.Holiday != null)
            {
                holidayRateDto.Holiday = HolidayDtoMapper.ToHolidayDTO(holidayRate.Holiday);
            }

            return holidayRateDto;
        }

        public static HolidayRate FromHolidayRateDto(HolidayRateDTO holidayRateDTO)
        {
            if (holidayRateDTO == null)
            {
                return null;
            }

            var holidayRate = new HolidayRate();
            holidayRate.Id = holidayRateDTO.Id;
            holidayRate.Rate = holidayRateDTO.Rate;

            if (holidayRateDTO.PetService != null)
            {
                holidayRate.PetService = PetServiceDtoMapper.FromPetServiceDTO(holidayRateDTO.PetService);
            }

            if (holidayRateDTO.Holiday != null)
            {
                holidayRate.Holiday = HolidayDtoMapper.FromHolidayDTO(holidayRateDTO.Holiday);
            }

            return holidayRate;
        }
    }
}
