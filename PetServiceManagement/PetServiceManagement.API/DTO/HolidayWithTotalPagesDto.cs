using System.Collections.Generic;

namespace PetServiceManagement.API.DTO
{
    public class HolidayWithTotalPagesDto
    {
        public HolidayWithTotalPagesDto(List<HolidayDTO> holidays, int totalPages)
        {
            Holidays = holidays;
            TotalPages = totalPages;
        }

        public List<HolidayDTO> Holidays { get; set; }

        public int TotalPages { get; set; }
    }
}
