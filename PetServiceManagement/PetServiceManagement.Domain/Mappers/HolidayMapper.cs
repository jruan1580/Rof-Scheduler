using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Collections.Generic;

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

        public static List<Holiday> ToHolidayDomains(List<Holidays> holidays)
        {
            if (holidays == null || holidays.Count == 0)
            {
                return new List<Holiday>();
            }

            var holidayDomains = new List<Holiday>();

            holidays.ForEach(holidayDb =>
            {
                var holiday = new Holiday();

                holiday.Id = holidayDb.Id;
                holiday.Name = holidayDb.HolidayName;
                holiday.HolidayMonth = holidayDb.HolidayMonth;
                holiday.HolidayDay = holidayDb.HolidayDay;

                holidayDomains.Add(holiday);
            });

            return holidayDomains;
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
