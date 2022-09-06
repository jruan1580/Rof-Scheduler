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
    public class PetServiceControllerTests
    {
        private Mock<IPetServiceManagementService> _petService;

        private PetServiceController _petServiceController;

        [SetUp]
        public void Setup()
        {
            _petService = new Mock<IPetServiceManagementService>();

            _petServiceController = new PetServiceController(_petService.Object);
        }

        [Test]
        public async Task GetByPageAndKeywordTest()
        {
            var petServices = new List<PetService>()
            {
                new PetService()
                {
                    Id = 1,
                    Name = "Dog Walking",
                    Description = "Walking dog",
                    Price = 20m,
                    EmployeeRate = 10m
                }
            };

            _petService.Setup(p => p.GetPetServicesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((petServices, 1));

            var res = await _petServiceController.GetByPageAndServiceName(1, 1, "walking");
            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;
            Assert.IsNotNull(okObj);
            Assert.AreEqual(200, okObj.StatusCode);

            Assert.AreEqual(typeof(List<PetServiceDTO>), okObj.Value.GetType());
            var petServicesDTO = (List<PetServiceDTO>)okObj.Value;

            Assert.AreEqual(1, petServicesDTO.Count);
            Assert.AreEqual(petServices[0].Id, petServicesDTO[0].Id);
            Assert.AreEqual(petServices[0].Name, petServicesDTO[0].Name);
            Assert.AreEqual(petServices[0].Price, petServicesDTO[0].Rate);
            Assert.AreEqual(petServices[0].Description, petServicesDTO[0].Description);
            Assert.AreEqual(petServices[0].EmployeeRate, petServicesDTO[0].EmployeeRate);
        }

        [Test]
        public async Task CreatePetServiceTest()
        {
            _petService.Setup(p => p.AddNewPetService(It.IsAny<PetService>()))
                .Returns(Task.CompletedTask);

            var res = await _petServiceController.AddPetService(new PetServiceDTO()
            {
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task CreatePetServiceBadRequestTest()
        {
            _petService.Setup(p => p.AddNewPetService(It.IsAny<PetService>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _petServiceController.AddPetService(new PetServiceDTO()
            {
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m
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
            _petService.Setup(p => p.UpdatePetService(It.IsAny<PetService>()))
                .Returns(Task.CompletedTask);

            var res = await _petServiceController.UpdatePetService(new PetServiceDTO()
            {
                Id = 1,
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m
            });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }

        [Test]
        public async Task UpdatePetServiceBadRequestTest()
        {
            _petService.Setup(p => p.UpdatePetService(It.IsAny<PetService>()))
               .ThrowsAsync(new ArgumentException("test"));

            var res = await _petServiceController.UpdatePetService(new PetServiceDTO()
            {
                Id = 1,
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m
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
            _petService.Setup(p => p.DeletePetServiceById(It.IsAny<short>()))
                .Returns(Task.CompletedTask);

            var res = await _petServiceController.DeletePetService(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());
        }
    }
}
