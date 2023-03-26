using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.Controllers
{
    [TestFixture]
    public class PetServiceControllerTests : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/PetService";

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
                    EmployeeRate = 10m,
                    Duration = 30,
                    TimeUnit = TimeUnits.MINUTES
                }
            };

            _petServiceManagementService.Setup(p => p.GetPetServicesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((petServices, 1));

            var url = $"{_baseUrl}?page=1&offset=1&keyword=cny";

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.GetAsync(url);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            Assert.IsNotNull(res.Content);
            var content = await res.Content.ReadAsStringAsync();

            try
            {
                var petServicesWithTotalPage = JsonConvert.DeserializeObject<PetServicesWithTotalPageDTO>(content);

                Assert.IsNotNull(petServicesWithTotalPage);

                var petServicesDTO = petServicesWithTotalPage.PetServices;

                Assert.AreEqual(1, petServicesDTO.Count);
                Assert.AreEqual(petServices[0].Id, petServicesDTO[0].Id);
                Assert.AreEqual(petServices[0].Name, petServicesDTO[0].Name);
                Assert.AreEqual(petServices[0].Price, petServicesDTO[0].Rate);
                Assert.AreEqual(petServices[0].Description, petServicesDTO[0].Description);
                Assert.AreEqual(petServices[0].EmployeeRate, petServicesDTO[0].EmployeeRate);
                Assert.AreEqual(petServices[0].Duration, petServicesDTO[0].Duration);
                Assert.AreEqual(petServices[0].TimeUnit, petServicesDTO[0].TimeUnit);

                Assert.AreEqual(1, petServicesWithTotalPage.TotalPages);

            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public async Task CreatePetServiceTest()
        {
            _petServiceManagementService.Setup(p => p.AddNewPetService(It.IsAny<PetService>()))
                .Returns(Task.CompletedTask);

            var dto = PetServiceFactory.GetPetServiceDTO();

            await SendNonGetAndDeleteRequestAndVerifySuccess(_baseUrl, "POST", dto);
        }

        [Test]
        public async Task CreatePetServiceBadRequestTest()
        {
            _petServiceManagementService.Setup(p => p.AddNewPetService(It.IsAny<PetService>()))
               .ThrowsAsync(new ArgumentException("test"));

            var dto = PetServiceFactory.GetPetServiceDTO();

            await SendNonGetAndDeleteRequestAndVerifyBadRequest(_baseUrl, "POST", dto, "test");
        }

        [Test]
        public async Task UpdatePetServiceTest()
        {
            _petServiceManagementService.Setup(p => p.UpdatePetService(It.IsAny<PetService>()))
                .Returns(Task.CompletedTask);

            var dto = PetServiceFactory.GetPetServiceDTO();

            SetAuthHeaderOnHttpClient("Administrator");

            await SendNonGetAndDeleteRequestAndVerifySuccess(_baseUrl, "PUT", dto);
        }

        [Test]
        public async Task UpdatePetServiceBadRequestTest()
        {
            _petServiceManagementService.Setup(p => p.UpdatePetService(It.IsAny<PetService>()))
               .ThrowsAsync(new ArgumentException("test"));

            var dto = PetServiceFactory.GetPetServiceDTO();

            await SendNonGetAndDeleteRequestAndVerifyBadRequest(_baseUrl, "PUT", dto, "test");
        }

        [Test]
        public async Task DeletePetServiceByIdTest()
        {
            _petServiceManagementService.Setup(p => p.DeletePetServiceById(It.IsAny<short>()))
                .Returns(Task.CompletedTask);

            var url = $"{_baseUrl}/1";

            await SendDeleteRequestAndVerifySuccess(url);
        }
    }
}
