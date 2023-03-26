using Moq;
using NUnit.Framework;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
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

            var holiday = new Holiday()
            {
                Id = 1,
                HolidayMonth = 1,
                HolidayDay = 28
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(holiday));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holiday));

            var invalidHolidayDate = new Holiday()
            {
                Id = 1,
                HolidayDay = 13,
                HolidayMonth = 13
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(holiday));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holiday));

            invalidHolidayDate = new Holiday()
            {
                Id = 1,
                HolidayDay = 33,
                HolidayMonth = 12
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.AddHoliday(holiday));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holiday));
        }

        [Test]
        public void UnableToFindHolidayForUpdateTest()
        {
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync((Holidays)null);

            var holiday = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayService.UpdateHoliday(holiday));
        }

        [Test]
        public async Task GetHolidaysByPageSuccessTest()
        {
            var holidays = new List<Holidays>()
            {
                new Holidays()
                {
                    Id = 1,
                    HolidayName = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                }
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
            var holiday = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };

            _holidayAndRateRepo.Setup(h => h.AddHoliday(It.IsAny<Holidays>())).ReturnsAsync((short)1);

            await _holidayService.AddHoliday(holiday);

            _holidayAndRateRepo.Verify(h => h.AddHoliday(It.IsAny<Holidays>()), Times.Once);
        }

        [Test]
        public async Task UpdateHolidayTest()
        {
            var holiday = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };

            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                   .ReturnsAsync(new Holidays()
                   {
                       Id = 1,
                       HolidayName = "Name to be updated",
                       HolidayMonth = 1,
                       HolidayDay = 28
                   });

            _holidayAndRateRepo.Setup(h => h.UpdateHoliday(It.IsAny<Holidays>())).Returns(Task.CompletedTask);

            await _holidayService.UpdateHoliday(holiday);

            _holidayAndRateRepo.Verify(p => p.UpdateHoliday(It.IsAny<Holidays>()), Times.Once);
        }
    }
}
