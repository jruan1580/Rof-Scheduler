using NUnit.Framework;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class HolidayMapperTests
    {
        [Test]
        public void MapToDomainHolidayTest()
        {
            var cny = DateTime.Today;
            var holidayEntity = new Holidays()
            {
                Id = 1,
                HolidayName = "CNY",
                HolidayDate = cny
            };

            var domainHoliday = HolidayMapper.ToHolidayDomain(holidayEntity);

            Assert.IsNotNull(domainHoliday);
            Assert.AreEqual(holidayEntity.Id, domainHoliday.Id);
            Assert.AreEqual(holidayEntity.HolidayName, domainHoliday.Name);
            Assert.AreEqual(holidayEntity.HolidayDate, domainHoliday.HolidayDate);
        }

        [Test]
        public void MapFromDomainHolidayTest()
        {
            var cny = DateTime.Today;
            var domainHoliday = new Holiday()
            {
                Id = 1,
                HolidayDate = cny,
                Name = "CNY"
            };

            var entityHoliday = HolidayMapper.FromHolidayDomain(domainHoliday);

            Assert.IsNotNull (entityHoliday);
            Assert.AreEqual(domainHoliday.Id, entityHoliday.Id);
            Assert.AreEqual(domainHoliday.Name, entityHoliday.HolidayName);
            Assert.AreEqual(domainHoliday.HolidayDate, entityHoliday.HolidayDate);
        }
    }
}
