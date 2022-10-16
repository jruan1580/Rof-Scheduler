using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PetServiceManagement.API.Controllers;
using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.Controllers
{
    [TestFixture]
    public class HolidayControllerTests
    {
        private Mock<IHolidayAndRateService> _holidayAndRateService;

        private HolidayController _holidayController;

        [SetUp]
        public void Setup()
        {
            _holidayAndRateService = new Mock<IHolidayAndRateService>();
            _holidayController = new HolidayController(_holidayAndRateService.Object);
        }

        [Test]
        public async Task GetByPageAndKeywordTest()
        {
            var holidays = new List<Holiday>()
            {
                new Holiday()
                {
                    Id = 1,
                    Name = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                }
            };

            _holidayAndRateService.Setup(h => h.GetHolidaysByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidays, 1));

            var res = await _holidayController.GetByPageAndHolidayName(1, 1, "cny");
            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;
            Assert.IsNotNull(okObj);
            Assert.AreEqual(200, okObj.StatusCode);

            Assert.AreEqual(typeof(HolidayWithTotalPagesDto), okObj.Value.GetType());
            var holidayWithTotalPagesDto = (HolidayWithTotalPagesDto)okObj.Value;

            var holidayDto = holidayWithTotalPagesDto.Holidays;

            Assert.IsNotNull(holidayDto);
            Assert.AreEqual(1, holidayDto.Count);
            Assert.AreEqual(holidays[0].Id, holidayDto[0].Id);
            Assert.AreEqual(holidays[0].Name, holidayDto[0].Name);
            Assert.AreEqual($"01/28/{DateTime.Now.Year}", holidayDto[0].Date);

            Assert.AreEqual(1, holidayWithTotalPagesDto.TotalPages);
        }

        [Test]
        public async Task CreateHolidayTest()
        {
            _holidayAndRateService.Setup(h => h.AddHoliday(It.IsAny<Holiday>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayController.AddHoliday(new HolidayDTO()
            {
                Name = "CNY",
                Month = 1,
                Day = 28
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task CreatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.AddHoliday(It.IsAny<Holiday>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _holidayController.AddHoliday(new HolidayDTO()
            {
                Name = "CNY",
                Month = 1,
                Day = 28
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(BadRequestObjectResult), res.GetType());

            var badRequestObj = (BadRequestObjectResult)res;
            Assert.AreEqual(typeof(string), badRequestObj.Value.GetType());
            Assert.AreEqual("test", badRequestObj.Value.ToString());
        }

        [Test]
        public async Task UpdatePetServiceTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHoliday(It.IsAny<Holiday>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayController.UpdateHoliday(new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task UpdatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHoliday(It.IsAny<Holiday>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _holidayController.UpdateHoliday(new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(BadRequestObjectResult), res.GetType());

            var badRequestObj = (BadRequestObjectResult)res;
            Assert.AreEqual(typeof(string), badRequestObj.Value.GetType());
            Assert.AreEqual("test", badRequestObj.Value.ToString());
        }

        [Test]
        public async Task DeletePetServiceByIdTest()
        {
            _holidayAndRateService.Setup(h => h.DeleteHolidayById(It.IsAny<short>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayController.DeleteHoliday(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }
    }
}
