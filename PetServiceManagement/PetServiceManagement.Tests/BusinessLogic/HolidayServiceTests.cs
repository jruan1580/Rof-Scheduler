using Moq;
using NUnit.Framework;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.BusinessLogic
{
    [TestFixture]
    public class HolidayServiceTests
    {
        private Mock<IHolidayAndRatesRepository> _holidayAndRateRepo = new Mock<IHolidayAndRatesRepository>();
        private IHolidayService _holidayService;

        [OneTimeSetUp]
        public void Setup()
        {
            _holidayService = new HolidayService(_holidayAndRateRepo.Object);
        }

        [Test]
        public void TestHolidayValidation()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(null));

            var holidayWithNoName = HolidayFactory.GetHolidayDomainObj();
            holidayWithNoName.Name = string.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(holidayWithNoName));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holidayWithNoName));

            var invalidHolidayDate = HolidayFactory.GetHolidayDomainObj();
            invalidHolidayDate.HolidayMonth = 13;

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(invalidHolidayDate));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(invalidHolidayDate));

            invalidHolidayDate.HolidayMonth = 12;
            invalidHolidayDate.HolidayDay = 33;

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(invalidHolidayDate));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(invalidHolidayDate));
        }

        [Test]
        public void UnableToFindHolidayForUpdateTest()
        {
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync((Holidays)null);

            var holiday = HolidayFactory.GetHolidayDomainObj();

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holiday));
        }

        [Test]
        public async Task GetHolidaysByPageSuccessTest()
        {
            var holidays = new List<Holidays>()
            {
                HolidayFactory.GetHolidayDbEntityObj()
            };

            _holidayAndRateRepo.Setup(h => h.GetHolidaysByPagesAndSearch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidays, 1));

            var result = await _holidayService.GetHolidaysByPageAndKeyword(1, 1, "CNY");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Item2);

            Assert.IsNotNull(result.Item1);
            Assert.AreEqual(1, result.Item1.Count);

            var holiday = result.Item1[0];

            Assert.AreEqual(holidays[0].Id, holiday.Id);
            Assert.AreEqual(holidays[0].HolidayName, holiday.Name);
            Assert.AreEqual(holidays[0].HolidayMonth, holiday.HolidayMonth);
        }

        [Test]
        public async Task AddHolidayTest()
        {
            var holiday = HolidayFactory.GetHolidayDomainObj();

            _holidayAndRateRepo.Setup(h => h.AddHoliday(It.IsAny<Holidays>())).ReturnsAsync((short)1);

            await _holidayService.AddHoliday(holiday);

            _holidayAndRateRepo.Verify(h => h.AddHoliday(It.IsAny<Holidays>()), Times.Once);
        }

        [Test]
        public async Task UpdateHolidayTest()
        {
            var holiday = HolidayFactory.GetHolidayDomainObj();

            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                   .ReturnsAsync(HolidayFactory.GetHolidayDbEntityObj());

            _holidayAndRateRepo.Setup(h => h.UpdateHoliday(It.IsAny<Holidays>())).Returns(Task.CompletedTask);

            await _holidayService.UpdateHoliday(holiday);

            _holidayAndRateRepo.Verify(p => p.UpdateHoliday(It.IsAny<Holidays>()), Times.Once);
        }
    }
}
