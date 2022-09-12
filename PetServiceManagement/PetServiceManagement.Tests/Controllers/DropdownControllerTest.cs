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
    public class DropdownControllerTest
    {
        private Mock<IDropDownService> _dropdownService;
        private DropdownController _dropdownController;

        [SetUp]
        public void Setup()
        {
            _dropdownService = new Mock<IDropDownService>();

            _dropdownController = new DropdownController(_dropdownService.Object);
        }

        [Test]
        public async Task GetPetServicesTest()
        {
            _dropdownService.Setup(d => d.GetDropdownForType<PetService>())
                .ReturnsAsync(new List<PetService>() 
                { 
                    new PetService()
                    {
                        Id = 1,
                        Name = "Dog Walking",
                        Description = "Walking dog",
                        Price = 20m,
                        EmployeeRate = 10m
                    }
                });

            var res = await _dropdownController.GetPetServices();

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;

            Assert.AreEqual(200, okObj.StatusCode);
            Assert.AreEqual(typeof(List<PetServiceDTO>), okObj.Value.GetType());

            var petServiceDtos = (List<PetServiceDTO>)okObj.Value;
            Assert.AreEqual(1, petServiceDtos.Count);

            var petServiceDto = petServiceDtos[0];
            Assert.AreEqual(1, petServiceDto.Id);
            Assert.AreEqual("Dog Walking", petServiceDto.Name);
            Assert.AreEqual("Walking dog", petServiceDto.Description);
            Assert.AreEqual(20m, petServiceDto.Rate);
            Assert.AreEqual(10m, petServiceDto.EmployeeRate);
        }

        [Test]
        public async Task GetHolidaysTest()
        {
            _dropdownService.Setup(d => d.GetDropdownForType<Holiday>())
                .ReturnsAsync(new List<Holiday>()
                {
                    new Holiday()
                    {
                        Id = 1,
                        Name = "CNY",
                        HolidayDate = new DateTime(2022, 9, 22)
                    }
                });


            var res = await _dropdownController.GetHolidays();

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;

            Assert.AreEqual(200, okObj.StatusCode);
            Assert.AreEqual(typeof(List<HolidayDTO>), okObj.Value.GetType());

            var holidayDtos = (List<HolidayDTO>)okObj.Value;
            Assert.AreEqual(1, holidayDtos.Count);

            var holidayDto = holidayDtos[0];
            Assert.AreEqual(1, holidayDto.Id);
            Assert.AreEqual("CNY", holidayDto.Name);
            Assert.AreEqual("09/22/2022", holidayDto.Date);
        }
    }
}
