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
    public class HolidayRateControllerTests
    {
        private Mock<IHolidayAndRateService> _holidayAndRateService;

        private HolidayRateController _holidayRateController;

        [SetUp]
        public void Setup()
        {
            _holidayAndRateService = new Mock<IHolidayAndRateService>();
            _holidayRateController = new HolidayRateController(_holidayAndRateService.Object);
        }

        [Test]
        public async Task GetByPageAndKeywordTest()
        {
            var holidayRates = new List<HolidayRate>()
            {
                new HolidayRate()
                {
                    Id = 1,
                    Rate = 20m,
                    PetService = new PetService()
                    {
                        Id = 1,
                        Name = "Dog Walking",
                        Description = "Dog Walking",
                        Price = 20m,
                        EmployeeRate = 10m
                    },
                    Holiday = new Holiday()
                    {
                        Id = 1,
                        Name = "CNY",
                        HolidayMonth = 1,
                        HolidayDay = 28
                    }
                }
            };

            _holidayAndRateService.Setup(h => h.GetHolidayRatesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidayRates, 1));

            var res = await _holidayRateController.GetByPageAndKeyword(1, 1, "cny");
            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;
            Assert.IsNotNull(okObj);
            Assert.AreEqual(200, okObj.StatusCode);

            Assert.AreEqual(typeof(List<HolidayRateDTO>), okObj.Value.GetType());
            var holidayRateDtos = (List<HolidayRateDTO>)okObj.Value;

            Assert.AreEqual(1, holidayRateDtos.Count);
            var holidayRateDto = holidayRateDtos[0];
            var holidayRate = holidayRates[0];

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
        public async Task CreateHolidayTest()
        {
            _holidayAndRateService.Setup(h => h.AddHolidayRate(It.IsAny<HolidayRate>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayRateController.AddHolidayRate(new HolidayRateDTO()
            {
                Rate = 20m,
                PetService = new PetServiceDTO()
                {
                    Id = 1,
                    Name = "Dog Walking",
                    Description = "Walking dog",
                    Rate = 20m,
                    EmployeeRate = 10m
                },
                Holiday = new HolidayDTO()
                {
                    Id = 1,
                    Name = "CNY",
                    Date = "09/06/2022"
                }
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task CreatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.AddHolidayRate(It.IsAny<HolidayRate>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _holidayRateController.AddHolidayRate(new HolidayRateDTO()
            {
                Rate = 20m,
                PetService = new PetServiceDTO()
                {
                    Id = 1,
                    Name = "Dog Walking",
                    Description = "Walking dog",
                    Rate = 20m,
                    EmployeeRate = 10m
                },
                Holiday = new HolidayDTO()
                {
                    Id = 1,
                    Name = "CNY",
                    Date = "09/06/2022"
                }
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
            _holidayAndRateService.Setup(h => h.UpdateHolidayRate(It.IsAny<HolidayRate>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayRateController.UpdateHolidayRate(new HolidayRateDTO()
            {
                Id = 1,
                Rate = 20m,
                PetService = new PetServiceDTO()
                {
                    Id = 1,
                    Name = "Dog Walking",
                    Description = "Walking dog",
                    Rate = 20m,
                    EmployeeRate = 10m
                },
                Holiday = new HolidayDTO()
                {
                    Id = 1,
                    Name = "CNY",
                    Date = "09/06/2022"
                }
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task UpdatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHolidayRate(It.IsAny<HolidayRate>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _holidayRateController.UpdateHolidayRate(new HolidayRateDTO()
            {
                Rate = 20m,
                PetService = new PetServiceDTO()
                {
                    Id = 1,
                    Name = "Dog Walking",
                    Description = "Walking dog",
                    Rate = 20m,
                    EmployeeRate = 10m
                },
                Holiday = new HolidayDTO()
                {
                    Id = 1,
                    Name = "CNY",
                    Date = "09/06/2022"
                }
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
            _holidayAndRateService.Setup(h => h.DeleteHolidayRateById(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var res = await _holidayRateController.DeleteHolidayRate(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }
    }
}
