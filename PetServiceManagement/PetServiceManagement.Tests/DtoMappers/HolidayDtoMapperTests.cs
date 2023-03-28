using NUnit.Framework;
using PetServiceManagement.API.DtoMapper;
using System;

namespace PetServiceManagement.Tests.DtoMappers
{
    [TestFixture]
    public class HolidayDtoMapperTests
    {
        [Test]
        public void ToHolidayDtoTest()
        {
            var domain = HolidayFactory.GetHolidayDomainObj();

            var dto = HolidayDtoMapper.ToHolidayDTO(domain);

            Assert.IsNotNull(dto);
            Assert.AreEqual(domain.Id, dto.Id);
            Assert.AreEqual(domain.Name, dto.Name);
            Assert.AreEqual($"01/28/{DateTime.Now.Year}", dto.Date);
        }

        [Test]
        public void FromHolidayDtoTest()
        {
            var dto = HolidayFactory.GetHolidayDTO();

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
