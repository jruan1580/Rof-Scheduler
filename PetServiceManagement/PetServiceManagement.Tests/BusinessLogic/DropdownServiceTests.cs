using Moq;
using NUnit.Framework;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.BusinessLogic
{
    [TestFixture]
    public class DropdownServiceTests
    {
        private Mock<IHolidayAndRatesRepository> _holidayAndRateRepo;
        private Mock<IPetServiceRepository> _petServiceRepo;
        private HolidayDropdownService _holidayDropdownService;
        private PetServiceDropdownService _petServiceDropdownService;

        [SetUp]
        public void Setup()
        {
            _holidayAndRateRepo = new Mock<IHolidayAndRatesRepository>();
            _petServiceRepo = new Mock<IPetServiceRepository>();

            _holidayAndRateRepo.Setup(h => h.GetAllHolidaysForDropdowns())
                .ReturnsAsync(new List<Holidays>()
                {
                    new Holidays()
                    {
                        Id = 1,
                        HolidayName = "CNY",
                        HolidayMonth = 1,
                        HolidayDay = 28
                    }
                });

            _petServiceRepo.Setup(p => p.GetAllPetServicesForDropdown())
                .ReturnsAsync(new List<PetServices>()
                {
                    new PetServices()
                    {
                        Id = 1,
                        ServiceName = "Dog Walking (30 Minutes)",
                        EmployeeRate = 20m,
                        Price = 20.99m,
                        Description = "Waling dog for 30 minutes"
                    }
                });

            _holidayDropdownService = new HolidayDropdownService(_holidayAndRateRepo.Object);
            _petServiceDropdownService = new PetServiceDropdownService(_petServiceRepo.Object);
        }

        [Test]
        public async Task GetPetServiceDropDownTest()
        {
            var petServicesDDL = await _petServiceDropdownService.GetDropdown();
            Assert.IsNotNull(petServicesDDL);
            Assert.AreEqual(1, petServicesDDL.Count);

            var petService = petServicesDDL[0];
            Assert.AreEqual(1, petService.Id);
            Assert.AreEqual("Dog Walking (30 Minutes)", petService.Name);
            Assert.AreEqual(20m, petService.EmployeeRate);
            Assert.AreEqual(20.99m, petService.Price);
            Assert.AreEqual("Waling dog for 30 minutes", petService.Description);
        }

        [Test]
        public async Task GetHolidaysDropdownTest()
        {
            var holidayDDL = await _holidayDropdownService.GetDropdown();

            Assert.IsNotNull(holidayDDL);
            Assert.AreEqual(1, holidayDDL.Count);

            var holiday = holidayDDL[0];
            Assert.AreEqual(1, holiday.Id);
            Assert.AreEqual("CNY", holiday.Name);
        }
    }
}
