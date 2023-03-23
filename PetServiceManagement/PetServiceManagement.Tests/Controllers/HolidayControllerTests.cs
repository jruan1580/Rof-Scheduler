using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.Controllers
{
    [TestFixture]
    public class HolidayControllerTests : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/Holiday";

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

            var url = $"{_baseUrl}?page=1&offset=1&keyword=cny";

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.GetAsync(url);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            Assert.IsNotNull(res.Content);
            var content = await res.Content.ReadAsStringAsync();

            try
            {
                var holidayDtos = JsonConvert.DeserializeObject<HolidayWithTotalPagesDto>(content);
                Assert.IsNotNull(holidayDtos);
                Assert.IsNotNull(holidayDtos.Holidays);
                Assert.AreEqual(1, holidayDtos.Holidays.Count);

                var holidayDto = holidayDtos.Holidays[0];

                Assert.AreEqual(holidays[0].Id, holidayDto.Id);
                Assert.AreEqual(holidays[0].Name, holidayDto.Name);
                Assert.AreEqual($"01/28/{DateTime.Now.Year}", holidayDto.Date);

                Assert.AreEqual(1, holidayDtos.TotalPages);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }           
        }

        [Test]
        public async Task CreateHolidayTest()
        {
            _holidayAndRateService.Setup(h => h.AddHoliday(It.IsAny<Holiday>()))
                .Returns(Task.CompletedTask);

            var dto = new HolidayDTO()
            {
                Name = "CNY",
                Month = 1,
                Day = 28
            };

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public async Task CreatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.AddHoliday(It.IsAny<Holiday>()))
               .ThrowsAsync(new ArgumentException("test"));

            var dto = new HolidayDTO()
            {
                Name = "CNY",
                Month = 1,
                Day = 28
            };

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);

            Assert.IsNotNull(res.Content);

            var content = await res.Content.ReadAsStringAsync();

            Assert.AreEqual("test", content);
        }

        [Test]
        public async Task UpdatePetServiceTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHoliday(It.IsAny<Holiday>()))
                .Returns(Task.CompletedTask);

            var dto = new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            };

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public async Task UpdatePetServiceBadRequestTest()
        {
            _holidayAndRateService.Setup(h => h.UpdateHoliday(It.IsAny<Holiday>()))
               .ThrowsAsync(new ArgumentException("test"));

            var dto = new HolidayDTO()
            {
                Id = 1,
                Name = "CNY",
                Month = 1,
                Day = 28
            };

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);

            Assert.IsNotNull(res.Content);

            var content = await res.Content.ReadAsStringAsync();

            Assert.AreEqual("test", content);
        }

        [Test]
        public async Task DeletePetServiceByIdTest()
        {
            _holidayAndRateService.Setup(h => h.DeleteHolidayById(It.IsAny<short>()))
                .Returns(Task.CompletedTask);

            var url = $"{_baseUrl}/1";

            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.DeleteAsync(url);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }
    }
}
