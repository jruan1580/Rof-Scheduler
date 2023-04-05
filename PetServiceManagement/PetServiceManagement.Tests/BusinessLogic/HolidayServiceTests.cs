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
        [Test]
        public void TestHolidayValidation()
        {
            var holidayService = new HolidayService(null, null);

            Assert.ThrowsAsync<ArgumentException>(() => holidayService.AddHoliday(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayService.UpdateHoliday(null));

            var holidayWithNoName = HolidayFactory.GetHolidayDomainObj();
            holidayWithNoName.Name = string.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => holidayService.AddHoliday(holidayWithNoName));
            Assert.ThrowsAsync<ArgumentException>(() => holidayService.UpdateHoliday(holidayWithNoName));

            var invalidHolidayDate = HolidayFactory.GetHolidayDomainObj();
            invalidHolidayDate.HolidayMonth = 13;

            Assert.ThrowsAsync<ArgumentException>(() => holidayService.AddHoliday(invalidHolidayDate));
            Assert.ThrowsAsync<ArgumentException>(() => holidayService.UpdateHoliday(invalidHolidayDate));

            invalidHolidayDate.HolidayMonth = 12;
            invalidHolidayDate.HolidayDay = 33;

            Assert.ThrowsAsync<ArgumentException>(() => holidayService.AddHoliday(invalidHolidayDate));
            Assert.ThrowsAsync<ArgumentException>(() => holidayService.UpdateHoliday(invalidHolidayDate));
        }

        [Test]
        public void UnableToFindHolidayForUpdateTest()
        {
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();

            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync((Holidays)null);

            var holidayService = new HolidayService(null, holidayRetrievalRepo.Object);

            var holiday = HolidayFactory.GetHolidayDomainObj();

            Assert.ThrowsAsync<ArgumentException>(() => holidayService.UpdateHoliday(holiday));
        }

        [Test]
        public async Task GetHolidaysByPageSuccessTest()
        {
            var holidays = new List<Holidays>()
            {
                HolidayFactory.GetHolidayDbEntityObj()
            };

            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();

            holidayRetrievalRepo.Setup(h => h.GetHolidaysByPagesAndSearch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidays, 1));

            var holidayService = new HolidayService(null, holidayRetrievalRepo.Object);

            var result = await holidayService.GetHolidaysByPageAndKeyword(1, 1, "CNY");

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

            var holidayUpsertRepo = new Mock<IHolidayUpsertRepository>();

            holidayUpsertRepo.Setup(h => h.AddHoliday(It.IsAny<Holidays>()))
                .ReturnsAsync((short)1);

            var holidayService = new HolidayService(holidayUpsertRepo.Object, null);

            await holidayService.AddHoliday(holiday);

            holidayUpsertRepo.Verify(h => h.AddHoliday(It.IsAny<Holidays>()), Times.Once);
        }

        [Test]
        public async Task UpdateHolidayTest()
        {
            var holiday = HolidayFactory.GetHolidayDomainObj();
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();
            var holidayUpsertRepo = new Mock<IHolidayUpsertRepository>();

            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                   .ReturnsAsync(HolidayFactory.GetHolidayDbEntityObj());

            holidayUpsertRepo.Setup(h => h.UpdateHoliday(It.IsAny<Holidays>()))
                .Returns(Task.CompletedTask);

            var holidayService = new HolidayService(holidayUpsertRepo.Object, holidayRetrievalRepo.Object);

            await holidayService.UpdateHoliday(holiday);

            holidayUpsertRepo.Verify(p => p.UpdateHoliday(It.IsAny<Holidays>()), Times.Once);
        }
    }
}
