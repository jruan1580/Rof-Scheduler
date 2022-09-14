using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Domain.Mappers
{
    public static class HolidayMapper
    {
        public static Holiday ToHolidayDomain(Holidays holidays)
        {
            if (holidays == null)
            {
                return null;
            }

            var holiday = new Holiday();

            holiday.Id = holidays.Id;
            holiday.Name = holidays.HolidayName;
            holiday.HolidayMonth = holidays.HolidayMonth;
            holiday.HolidayDay = holidays.HolidayDay;

            return holiday;
        }

        public static Holidays FromHolidayDomain(Holiday holiday)
        {
            if (holiday == null)
            {
                return null;
            }

            Holidays holidays = new Holidays();

            holidays.Id = holiday.Id;
            holidays.HolidayName = holiday.Name;
            holidays.HolidayMonth = holiday.HolidayMonth;
            holidays.HolidayDay = holiday.HolidayDay;

            return holidays;
        }
    }
}
