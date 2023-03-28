using NUnit.Framework;
using PetServiceManagement.Domain.Mappers;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class HolidayRateMapperTests
    {
        [Test]
        public void MapToDomainHolidayRateTest()
        {
            var holidayRateEntity = HolidayRateFactory.GetHoldayRateDbEntity();

            var holidayRateDomain = HolidayRatesMapper.ToDomainHolidayRate(holidayRateEntity);

            Assert.IsNotNull(holidayRateDomain);
            Assert.AreEqual(holidayRateEntity.Id, holidayRateDomain.Id);
            Assert.AreEqual(holidayRateEntity.HolidayRate, holidayRateDomain.Rate);

            Assert.IsNotNull(holidayRateDomain.PetService);
            Assert.AreEqual(holidayRateEntity.PetServiceId, holidayRateDomain.PetService.Id);
            Assert.AreEqual(holidayRateEntity.PetService.ServiceName, holidayRateDomain.PetService.Name);
            Assert.AreEqual(holidayRateEntity.PetService.Description, holidayRateDomain.PetService.Description);
            Assert.AreEqual(holidayRateEntity.PetService.Price, holidayRateDomain.PetService.Price);
            Assert.AreEqual(holidayRateEntity.PetService.EmployeeRate, holidayRateDomain.PetService.EmployeeRate);

            Assert.IsNotNull(holidayRateDomain.Holiday);
            Assert.AreEqual(holidayRateEntity.HolidayId, holidayRateDomain.Holiday.Id);
            Assert.AreEqual(holidayRateEntity.Holiday.HolidayName, holidayRateDomain.Holiday.Name);
            Assert.AreEqual(holidayRateEntity.Holiday.HolidayMonth, holidayRateDomain.Holiday.HolidayMonth);
        }

        [Test]
        public void MapFromDomainHolidayRateTest()
        {
            var holidayRateDomain = HolidayRateFactory.GetHolidayRateDomainObj();

            var holidayRateEntity = HolidayRatesMapper.FromDomainHolidayRate(holidayRateDomain);

            Assert.IsNotNull(holidayRateEntity);
            Assert.AreEqual(holidayRateDomain.Id, holidayRateEntity.Id);
            Assert.AreEqual(holidayRateDomain.Rate, holidayRateEntity.HolidayRate);
            Assert.AreEqual(holidayRateDomain.PetService.Id, holidayRateEntity.PetServiceId);
            Assert.AreEqual(holidayRateDomain.Holiday.Id, holidayRateEntity.HolidayId);
        }
    }
}
