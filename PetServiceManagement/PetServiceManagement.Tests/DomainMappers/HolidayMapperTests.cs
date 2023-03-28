using NUnit.Framework;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Collections.Generic;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class HolidayMapperTests
    {
        [Test]
        public void MapToDomainHolidayTest()
        {
            var holidayEntity = HolidayFactory.GetHolidayDbEntityObj();

            var domainHoliday = HolidayMapper.ToHolidayDomain(holidayEntity);

            Assert.IsNotNull(domainHoliday);
            Assert.AreEqual(holidayEntity.Id, domainHoliday.Id);
            Assert.AreEqual(holidayEntity.HolidayName, domainHoliday.Name);
            Assert.AreEqual(holidayEntity.HolidayMonth, domainHoliday.HolidayMonth);
            Assert.AreEqual(holidayEntity.HolidayDay, domainHoliday.HolidayDay);
        }

        [Test]
        public void MapToDomainHolidaysTest()
        {
            var holidayEntities = new List<Holidays>()
            {
                HolidayFactory.GetHolidayDbEntityObj()
            };

            var domainHolidays = HolidayMapper.ToHolidayDomains(holidayEntities);

            Assert.IsNotNull(domainHolidays);
            Assert.AreEqual(1, holidayEntities.Count);

            var domainHoliday = domainHolidays[0];
            var holidayEntity = holidayEntities[0];

            Assert.IsNotNull(domainHoliday);
            Assert.AreEqual(holidayEntity.Id, domainHoliday.Id);
            Assert.AreEqual(holidayEntity.HolidayName, domainHoliday.Name);
            Assert.AreEqual(holidayEntity.HolidayMonth, domainHoliday.HolidayMonth);
            Assert.AreEqual(holidayEntity.HolidayDay, domainHoliday.HolidayDay);
        }

        [Test]
        public void MapFromDomainHolidayTest()
        {
            var domainHoliday = HolidayFactory.GetHolidayDomainObj();

            var entityHoliday = HolidayMapper.FromHolidayDomain(domainHoliday);

            Assert.IsNotNull (entityHoliday);
            Assert.AreEqual(domainHoliday.Id, entityHoliday.Id);
            Assert.AreEqual(domainHoliday.Name, entityHoliday.HolidayName);
            Assert.AreEqual(domainHoliday.HolidayMonth, entityHoliday.HolidayMonth);
            Assert.AreEqual(domainHoliday.HolidayDay, entityHoliday.HolidayDay);
        }
    }
}
