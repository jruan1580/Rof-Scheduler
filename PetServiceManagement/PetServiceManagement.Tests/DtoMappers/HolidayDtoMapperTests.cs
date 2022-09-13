using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.Models;
using System;

namespace PetServiceManagement.Tests.DtoMappers
{
    [TestFixture]
    public class HolidayDtoMapperTests
    {
        [Test]
        public void ToHolidayDtoTest()
        {
            var domain = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayDay = 28,
                HolidayMonth = 1
            };

            var dto = HolidayDtoMapper.ToHolidayDTO(domain);

            Assert.IsNotNull(dto);
            Assert.AreEqual(domain.Id, dto.Id);
            Assert.AreEqual(domain.Name, dto.Name);
            Assert.AreEqual($"01/28/{DateTime.Now.Year}", dto.Date);
        }

        [Test]
        public void FromHolidayDtoTest()
        {
            var dto = new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            };

            var domain = HolidayDtoMapper.FromHolidayDTO(dto);
            Assert.IsNotNull(domain);
            Assert.AreEqual(dto.Id, domain.Id);
            Assert.AreEqual(dto.Name, domain.Name);

            var date = DateTime.Parse(dto.Date);

            Assert.AreEqual(date.Month, domain.HolidayMonth);
            Assert.AreEqual(date.Day, domain.HolidayDay);
        }
    }
}
