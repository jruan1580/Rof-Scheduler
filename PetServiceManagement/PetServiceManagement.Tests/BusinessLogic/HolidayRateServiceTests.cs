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
        [Test]
        public void TestHolidayRatesValidation()
        {
            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();          
            var holidayRateService = new HolidayRateService(petServiceRepo.Object, 
                holidayRetrievalRepo.Object, 
                null,
                null);

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));

            //no pet service or holiday defined
            var holidayRate = new HolidayRate();
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));

            //define pet service but not holiday
            holidayRate.PetService = PetServiceFactory.GetPetServiceDomain();

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));

            //define holiday but not pet service
            holidayRate.PetService = null;
            holidayRate.Holiday = HolidayFactory.GetHolidayDomainObj();

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));

            //redefine pet service
            holidayRate.PetService = PetServiceFactory.GetPetServiceDomain();

            //unable to find pet service
            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync((PetServices)null);

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));

            //finds pet service but not holiday
            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync((Holidays)null);

            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.AddHolidayRate(null));
        }

        [Test]
        public void UnableToFindHolidayRatesForUpdateTest()
        {
            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();
            var holidayRateRetrievalRepo = new Mock<IHolidayRateRetrievalRepository>();
            var holidayRateService = new HolidayRateService(petServiceRepo.Object, 
                holidayRetrievalRepo.Object, null,
                holidayRateRetrievalRepo.Object);            

            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(HolidayFactory.GetHolidayDbEntityObj());

            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            holidayRateRetrievalRepo.Setup(h => h.GetHolidayRatesById(It.IsAny<int>()))
                .ReturnsAsync((HolidayRates)null);

            var holidayRate = HolidayRateFactory.GetHolidayRateDomainObj();

            Assert.ThrowsAsync<ArgumentException>(() => holidayRateService.UpdateHolidayRate(holidayRate));
        }

        [Test]
        public async Task GetHolidayRatesByPageAndKeywordTest()
        {
            var holidayRateRetrievalRepo = new Mock<IHolidayRateRetrievalRepository>();
            var holidayRateService = new HolidayRateService(null, null, null, holidayRateRetrievalRepo.Object);

            var holidayRates = new List<HolidayRates>
            {
                HolidayRateFactory.GetHoldayRateDbEntity()
            };

            holidayRateRetrievalRepo.Setup(h => h.GetHolidayRatesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((holidayRates, 1));

            var result = await holidayRateService.GetHolidayRatesByPageAndKeyword(1, 1, "CNY");
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
            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();
            var holidayUpsertRepo = new Mock<IHolidayRateUpsertRepository>();
            var holidayRateService = new HolidayRateService(petServiceRepo.Object, holidayRetrievalRepo.Object, holidayUpsertRepo.Object, null);

            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(HolidayFactory.GetHolidayDbEntityObj());

            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            var holidayRate = HolidayRateFactory.GetHolidayRateDomainObj();

            holidayUpsertRepo.Setup(h => h.CreateHolidayRates(It.IsAny<HolidayRates>())).ReturnsAsync(1);

            await holidayRateService.AddHolidayRate(holidayRate);

            holidayUpsertRepo.Verify(h => h.CreateHolidayRates(It.IsAny<HolidayRates>()), Times.Once);
        }

        [Test]
        public async Task UpdateHolidayRateTest()
        {
            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();
            var holidayRetrievalRepo = new Mock<IHolidayRetrievalRepository>();
            var holidayRateRetrievalRepo = new Mock<IHolidayRateRetrievalRepository>();
            var holidayUpsertRepo = new Mock<IHolidayRateUpsertRepository>();

            var holidayRateService = new HolidayRateService(petServiceRepo.Object, 
                holidayRetrievalRepo.Object,
                holidayUpsertRepo.Object, 
                holidayRateRetrievalRepo.Object);

            holidayRetrievalRepo.Setup(h => h.GetHolidayById(It.IsAny<short>()))
                .ReturnsAsync(HolidayFactory.GetHolidayDbEntityObj());

            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
              .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            var holidayRate = HolidayRateFactory.GetHolidayRateDomainObj();

            holidayRateRetrievalRepo.Setup(h => h.GetHolidayRatesById(It.IsAny<int>()))
                   .ReturnsAsync(new HolidayRates()
                   {
                       Id = 1,
                       PetServiceId = 2,
                       HolidayId = 2,
                       HolidayRate = 20m
                   });

            holidayUpsertRepo.Setup(h => h.UpdateHolidayRates(It.IsAny<HolidayRates>())).Returns(Task.CompletedTask);

            await holidayRateService.UpdateHolidayRate(holidayRate);

            holidayUpsertRepo.Verify(p => p.UpdateHolidayRates(It.IsAny<HolidayRates>()), Times.Once);
        }
    }
}
