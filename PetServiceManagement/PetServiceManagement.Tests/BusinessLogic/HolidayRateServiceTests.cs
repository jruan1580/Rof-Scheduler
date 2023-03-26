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
    public class HolidayRateServiceTests
    {
        private Mock<IPetServiceRepository> _petServiceRepo;
        private Mock<IHolidayAndRatesRepository> _holidayAndRateRepo;

        private IHolidayRateService _holidayRateService;

        [OneTimeSetUp]
        public void Setup()
        {
            _petServiceRepo = new Mock<IPetServiceRepository>();
            _holidayAndRateRepo = new Mock<IHolidayAndRatesRepository>();

            _holidayRateService = new HolidayRateService(_petServiceRepo.Object, _holidayAndRateRepo.Object);
        }

        [Test]
        public void TestHolidayRatesValidation()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));

            //no pet service or holiday defined
            var holidayRate = new HolidayRate();
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));

            //define pet service but not holiday
            holidayRate.PetService = new PetService()
            {
                Id = 1,
                Price = 20m,
                Name = "Dog Walking",
                Description = "Walking dog"
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));

            //define holiday but not pet service
            holidayRate.PetService = null;
            holidayRate.Holiday = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));

            //redefine pet service
            holidayRate.PetService = new PetService()
            {
                Id = 1,
                Price = 20m,
                Name = "Dog Walking",
                Description = "Walking dog"
            };

            //unable to find pet service
            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync((PetServices)null);

            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));

            //finds pet service but not holiday
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync((Holidays)null);

            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(new PetServices()
                {
                    Id = 1,
                    Price = 20m,
                    ServiceName = "Dog Walking",
                    Description = "Walking dog"
                });

            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.AddHolidayRate(null));
        }

        [Test]
        public void UnableToFindHolidayRatesForUpdateTest()
        {
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(new Holidays()
                {
                    Id = 1,
                    HolidayName = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                });

            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(new PetServices()
              {
                  Id = 1,
                  Price = 20m,
                  ServiceName = "Dog Walking",
                  Description = "Walking dog"
              });

            _holidayAndRateRepo.Setup(h => h.GetHolidayRatesById(It.IsAny<int>()))
                .ReturnsAsync((HolidayRates)null);

            var holidayRate = new HolidayRate()
            {
                Id = 1,
                PetService = new PetService()
                {
                    Id = 1,
                    Price = 20m,
                    Name = "Dog Walking",
                    Description = "Walking dog"
                },
                Holiday = new Holiday()
                {
                    Id = 1,
                    Name = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                },
                Rate = 20m
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayRateService.UpdateHolidayRate(holidayRate));
        }

        [Test]
        public async Task GetHolidayRatesByPageAndKeywordTest()
        {
            var holidayRates = new List<HolidayRates>
            {
                new HolidayRates()
                {
                    Id = 1,
                    PetServiceId = 1,
                    HolidayId = 1,
                    PetService = new PetServices()
                    {
                        Id = 1,
                        Price = 20m,
                        ServiceName = "Dog Walking",
                        Description = "Walking dog"
                    },
                    Holiday = new Holidays()
                    {
                        Id = 1,
                        HolidayName = "CNY",
                        HolidayMonth = 1,
                        HolidayDay = 28
                    },
                    HolidayRate = 20m
                }
            };

            _holidayAndRateRepo.Setup(h => h.GetHolidayRatesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidayRates, 1));

            var result = await _holidayRateService.GetHolidayRatesByPageAndKeyword(1, 1, "CNY");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Item2);

            Assert.IsNotNull(result.Item1);
            Assert.AreEqual(1, result.Item1.Count);

            var holidayRate = result.Item1[0];

            Assert.AreEqual(holidayRates[0].Id, holidayRate.Id);

            Assert.IsNotNull(holidayRate.PetService);
            Assert.AreEqual(holidayRates[0].PetService.Id, holidayRate.PetService.Id);
            Assert.AreEqual(holidayRates[0].PetService.ServiceName, holidayRate.PetService.Name);
            Assert.AreEqual(holidayRates[0].PetService.Price, holidayRate.PetService.Price);
            Assert.AreEqual(holidayRates[0].PetService.Description, holidayRate.PetService.Description);

            Assert.IsNotNull(holidayRate.Holiday);
            Assert.AreEqual(holidayRates[0].Holiday.Id, holidayRate.Holiday.Id);
            Assert.AreEqual(holidayRates[0].Holiday.HolidayName, holidayRate.Holiday.Name);
            Assert.AreEqual(holidayRates[0].Holiday.HolidayMonth, holidayRate.Holiday.HolidayMonth);
            Assert.AreEqual(holidayRates[0].Holiday.HolidayDay, holidayRate.Holiday.HolidayDay);
        }


        [Test]
        public async Task AddHolidayRateTest()
        {
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(new Holidays()
                {
                    Id = 1,
                    HolidayName = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                });

            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(new PetServices()
              {
                  Id = 1,
                  Price = 20m,
                  ServiceName = "Dog Walking",
                  Description = "Walking dog"
              });

            var holidayRate = new HolidayRate()
            {
                Id = 1,
                PetService = new PetService()
                {
                    Id = 1,
                    Price = 20m,
                    Name = "Dog Walking",
                    Description = "Walking dog"
                },
                Holiday = new Holiday()
                {
                    Id = 1,
                    Name = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                },
                Rate = 30m
            };

            _holidayAndRateRepo.Setup(h => h.CreateHolidayRates(It.IsAny<HolidayRates>())).ReturnsAsync(1);

            await _holidayRateService.AddHolidayRate(holidayRate);

            _holidayAndRateRepo.Verify(h => h.CreateHolidayRates(It.IsAny<HolidayRates>()), Times.Once);
        }

        [Test]
        public async Task UpdateHolidayRateTest()
        {
            _holidayAndRateRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(new Holidays()
                {
                    Id = 1,
                    HolidayName = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                });

            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(new PetServices()
              {
                  Id = 1,
                  Price = 20m,
                  ServiceName = "Dog Walking",
                  Description = "Walking dog"
              });

            var holidayRate = new HolidayRate()
            {
                Id = 1,
                PetService = new PetService()
                {
                    Id = 1,
                    Price = 20m,
                    Name = "Dog Walking",
                    Description = "Walking dog"
                },
                Holiday = new Holiday()
                {
                    Id = 1,
                    Name = "CNY",
                    HolidayMonth = 1,
                    HolidayDay = 28
                },
                Rate = 30m
            };

            _holidayAndRateRepo.Setup(h => h.GetHolidayRatesById(It.IsAny<int>()))
                   .ReturnsAsync(new HolidayRates()
                   {
                       Id = 1,
                       PetServiceId = 2,
                       HolidayId = 2,
                       HolidayRate = 20m
                   });

            _holidayAndRateRepo.Setup(h => h.UpdateHolidayRates(It.IsAny<HolidayRates>())).Returns(Task.CompletedTask);

            await _holidayRateService.UpdateHolidayRate(holidayRate);

            _holidayAndRateRepo.Verify(p => p.UpdateHolidayRates(It.IsAny<HolidayRates>()), Times.Once);
        }
    }
}
