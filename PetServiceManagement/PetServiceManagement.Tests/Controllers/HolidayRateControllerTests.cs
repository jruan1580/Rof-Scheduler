using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.Controllers
{
    [TestFixture]
    public class HolidayRateControllerTests : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/HolidayRate";

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

            var url = $"{_baseUrl}?page=1&offset=1&keyword=cny";
            
            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.GetAsync(url);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            Assert.IsNotNull(res.Content);
            var content = await res.Content.ReadAsStringAsync();

            try
            {
                var holidayRateDtos = JsonConvert.DeserializeObject<List<HolidayRateDTO>>(content);

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
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }            
        }

        [Test]
        public async Task CreateHolidayTest()
        {
            _holidayAndRateService.Setup(h => h.AddHolidayRate(It.IsAny<HolidayRate>()))
                .Returns(Task.CompletedTask);

            var holidayRateDTO = GetHolidayRateDTO();

            await SendNonGetAndDeleteRequestAndVerifySuccess(_baseUrl, "POST", holidayRateDTO);
        }

        [Test]
        public async Task CreateHolidayBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.AddHolidayRate(It.IsAny<HolidayRate>()))
               .ThrowsAsync(new ArgumentException("test"));

            var holidayRateDTO = GetHolidayRateDTO();

            await SendNonGetAndDeleteRequestAndVerifyBadRequest(_baseUrl, "POST", holidayRateDTO, "test");
        }

        [Test]
        public async Task UpdateHolidayRateTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHolidayRate(It.IsAny<HolidayRate>()))
                .Returns(Task.CompletedTask);

            var holidayRateDTO = GetHolidayRateDTO();

            await SendNonGetAndDeleteRequestAndVerifySuccess(_baseUrl, "PUT", holidayRateDTO);
        }

        [Test]
        public async Task UpdateHolidayRateBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHolidayRate(It.IsAny<HolidayRate>()))
               .ThrowsAsync(new ArgumentException("test"));

            var holidayRateDTO = GetHolidayRateDTO();

            await SendNonGetAndDeleteRequestAndVerifyBadRequest(_baseUrl, "PUT", holidayRateDTO, "test");
        }

        [Test]
        public async Task DeleteHolidayRateByIdTest()
        {
            _holidayAndRateService.Setup(h => h.DeleteHolidayRateById(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var url = $"{_baseUrl}/1";

            await SendDeleteRequestAndVerifySuccess(url);
        }

        private HolidayRateDTO GetHolidayRateDTO()
        {
            return new HolidayRateDTO()
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
                    Month = 1,
                    Day = 28
                }
            };
        }
    }
}
