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
                HolidayDate = DateTime.Now
            };

            var dto = HolidayDtoMapper.ToHolidayDTO(domain);

            Assert.IsNotNull(dto);
            Assert.AreEqual(domain.Id, dto.Id);
            Assert.AreEqual(domain.Name, dto.Name);
            Assert.AreEqual(domain.HolidayDate.ToString("MM/dd/yyyy"), dto.Date);
        }

        [Test]
        public void FromHolidayDtoTest()
        {
            var dto = new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Date = "09/06/2022"
            };

            var domain = HolidayDtoMapper.FromHolidayDTO(dto);
            Assert.IsNotNull(domain);
            Assert.AreEqual(dto.Id, domain.Id);
            Assert.AreEqual(dto.Name, domain.Name);
            Assert.AreEqual(DateTime.Parse(dto.Date), domain.HolidayDate);
        }
    }
}
