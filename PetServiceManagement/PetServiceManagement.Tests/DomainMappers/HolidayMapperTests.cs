using NUnit.Framework;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class HolidayMapperTests
    {
        [Test]
        public void MapToDomainHolidayTest()
        {
            var holidayEntity = new Holidays()
            {
                Id = 1,
                HolidayName = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };

            var domainHoliday = HolidayMapper.ToHolidayDomain(holidayEntity);

            Assert.IsNotNull(domainHoliday);
            Assert.AreEqual(holidayEntity.Id, domainHoliday.Id);
            Assert.AreEqual(holidayEntity.HolidayName, domainHoliday.Name);
            Assert.AreEqual(holidayEntity.HolidayMonth, domainHoliday.HolidayMonth);
            Assert.AreEqual(holidayEntity.HolidayDay, domainHoliday.HolidayDay);
        }

        [Test]
        public void MapFromDomainHolidayTest()
        {
            var domainHoliday = new Holiday()
            {
                Id = 1,
                HolidayMonth = 1,
                HolidayDay = 28,
                Name = "CNY"
            };

            var entityHoliday = HolidayMapper.FromHolidayDomain(domainHoliday);

            Assert.IsNotNull (entityHoliday);
            Assert.AreEqual(domainHoliday.Id, entityHoliday.Id);
            Assert.AreEqual(domainHoliday.Name, entityHoliday.HolidayName);
            Assert.AreEqual(domainHoliday.HolidayMonth, entityHoliday.HolidayMonth);
            Assert.AreEqual(domainHoliday.HolidayDay, entityHoliday.HolidayDay);
        }
    }
}
