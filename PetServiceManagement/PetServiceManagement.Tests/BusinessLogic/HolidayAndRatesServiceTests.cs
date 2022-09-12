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
    public class HolidayAndRatesServiceTests
    {
        private Mock<IPetServiceRepository> _petServiceRepo;
        private Mock<IHolidayAndRatesRepository> _holidayAndRateRepo;
        private HolidayAndRateService _holidayAndRateService;

        [SetUp]
        public void Setup()
        {
            _petServiceRepo = new Mock<IPetServiceRepository>();
            _holidayAndRateRepo = new Mock<IHolidayAndRatesRepository>();

            _holidayAndRateService = new HolidayAndRateService(_petServiceRepo.Object, _holidayAndRateRepo.Object);
        }

        [Test]
        public void TestHolidayValidation()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHoliday(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.UpdateHoliday(null));

            var holiday = new Holiday()
            {
                Id = 1,
                HolidayMonth = 1,
                HolidayDay = 28
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHoliday(holiday));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.UpdateHoliday(holiday));
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

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.UpdateHoliday(holiday));
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

            var result = await _holidayAndRateService.GetHolidaysByPageAndKeyword(1, 1, "CNY");
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

            await _holidayAndRateService.AddHoliday(holiday);

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

            await _holidayAndRateService.UpdateHoliday(holiday);

            _holidayAndRateRepo.Verify(p => p.UpdateHoliday(It.IsAny<Holidays>()), Times.Once);
        }

        [Test]
        public void TestHolidayRatesValidation()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));

            //no pet service or holiday defined
            var holidayRate = new HolidayRate();
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));

            //define pet service but not holiday
            holidayRate.PetService = new PetService()
            {
                Id = 1,
                Price = 20m,
                Name = "Dog Walking",
                Description = "Walking dog"
            };

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));

            //define holiday but not pet service
            holidayRate.PetService = null;
            holidayRate.Holiday = new Holiday()
            {
                Id = 1,
                Name = "CNY",
                HolidayMonth = 1,
                HolidayDay = 28
            };
            
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));

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

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));

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

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.AddHolidayRate(null));
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

            Assert.ThrowsAsync<ArgumentException>(() => _holidayAndRateService.UpdateHolidayRate(holidayRate));
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

            var result = await _holidayAndRateService.GetHolidayRatesByPageAndKeyword(1, 1, "CNY");
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

            await _holidayAndRateService.AddHolidayRate(holidayRate);

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

            await _holidayAndRateService.UpdateHolidayRate(holidayRate);

            _holidayAndRateRepo.Verify(p => p.UpdateHolidayRates(It.IsAny<HolidayRates>()), Times.Once);
        }
    }
}
