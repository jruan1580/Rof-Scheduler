using NUnit.Framework;
using PetServiceManagement.API.DtoMapper;
using System;

namespace PetServiceManagement.Tests.DtoMappers
{
    [TestFixture]
    public class HolidayRateDtoMapperTests
    {
        [Test]
        public void ToHolidayRateDtoTest()
        {
            var holidayRate = HolidayRateFactory.GetHolidayRateDomainObj();

            var holidayRateDto = HolidayRateDtoMapper.ToHolidayRateDto(holidayRate);

            Assert.IsNotNull(holidayRateDto);
            Assert.AreEqual(holidayRate.Id, holidayRateDto.Id);
            Assert.AreEqual(holidayRate.Rate, holidayRateDto.Rate);

            Assert.IsNotNull(holidayRateDto.PetService);
            Assert.AreEqual(holidayRate.PetService.Id, holidayRateDto.PetService.Id);
            Assert.AreEqual(holidayRate.PetService.Name, holidayRateDto.PetService.Name);
            Assert.AreEqual(holidayRate.PetService.Price, holidayRateDto.PetService.Rate);
            Assert.AreEqual(holidayRate.PetService.EmployeeRate, holidayRateDto.PetService.EmployeeRate);
            Assert.AreEqual(holidayRate.PetService.Description, holidayRateDto.PetService.Description);

            Assert.IsNotNull(holidayRateDto.Holiday);
            Assert.AreEqual(holidayRate.Holiday.Id, holidayRateDto.Holiday.Id);
            Assert.AreEqual(holidayRate.Holiday.Name, holidayRateDto.Holiday.Name);
            Assert.AreEqual($"01/28/{DateTime.Now.Year}", holidayRateDto.Holiday.Date);
        }

        [Test]
        public void FromHolidayRateDtoTest()
        {
            var holidayRateDto = HolidayRateFactory.GetHolidayRateDTO();

            var holidayRate = HolidayRateDtoMapper.FromHolidayRateDto(holidayRateDto);

            Assert.IsNotNull(holidayRate);
            Assert.AreEqual(holidayRateDto.Id, holidayRate.Id);
            Assert.AreEqual(holidayRateDto.Rate, holidayRate.Rate);

            Assert.IsNotNull(holidayRate.PetService);
            Assert.AreEqual(holidayRateDto.PetService.Id, holidayRate.PetService.Id);
            Assert.AreEqual(holidayRateDto.PetService.Name, holidayRate.PetService.Name);
            Assert.AreEqual(holidayRateDto.PetService.Rate, holidayRate.PetService.Price);
            Assert.AreEqual(holidayRateDto.PetService.EmployeeRate, holidayRate.PetService.EmployeeRate);
            Assert.AreEqual(holidayRateDto.PetService.Description, holidayRate.PetService.Description);

            Assert.IsNotNull(holidayRate.Holiday);
            Assert.AreEqual(holidayRateDto.Holiday.Id, holidayRate.Holiday.Id);
            Assert.AreEqual(holidayRateDto.Holiday.Name, holidayRate.Holiday.Name);

            var holidayDate = DateTime.Parse(holidayRateDto.Holiday.Date);
            Assert.AreEqual(holidayDate.Month, holidayRate.Holiday.HolidayMonth);
            Assert.AreEqual(holidayDate.Day, holidayRate.Holiday.HolidayDay);
        }
    }
}
