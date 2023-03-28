using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Tests
{
    public static class HolidayFactory
    {
        public static Holiday GetHolidayDomainObj()
        {
            return new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayDay = 28,
                HolidayMonth = 1
            };
        }

        public static Holidays GetHolidayDbEntityObj()
        {
            return new Holidays()
            {
                Id = 1,
                HolidayName = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };
        }

        public static HolidayDTO GetHolidayDTO()
        {
            return new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            };
        }
    }
}
