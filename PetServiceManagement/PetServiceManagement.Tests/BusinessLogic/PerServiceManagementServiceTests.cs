using Moq;
using NUnit.Framework;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.BusinessLogic
{
    [TestFixture]
    public class PerServiceManagementServiceTests
    {
        private Mock<IPetServiceRepository> _petServiceRepo;
        private PetServiceManagementService _petServiceManagementService;

        [SetUp]
        public void Setup()
        {
            _petServiceRepo = new Mock<IPetServiceRepository>();

            _petServiceManagementService = new PetServiceManagementService(_petServiceRepo.Object);
        }

        [Test]
        public void PetServiceValidationNoNameTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(null));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(null));

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Name = string.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidPrice()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Price = -1m;

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidEmployeeRate()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.EmployeeRate = -1m;

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));

            petService.EmployeeRate = 101m;

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidDuration()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Duration = -1;

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidTimeUnit()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.TimeUnit = "Sec";

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void UnableToFindForUpdateTest()
        {
            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync((PetServices)null);

            var petService = PetServiceFactory.GetPetServiceDomain();

            Assert.ThrowsAsync<ArgumentException>(() => _petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public async Task GetByPageSuccessTest()
        {
            var petServices = new List<PetServices>()
            {
                PetServiceFactory.GetPetServicesDbEntity(),
            };

            _petServiceRepo.Setup(p => p.GetAllPetServicesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((petServices, 1));

            var result = await _petServiceManagementService.GetPetServicesByPageAndKeyword(1, 1, "Walking");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Item2);

            Assert.IsNotNull(result.Item1);
            Assert.AreEqual(1, result.Item1.Count);

            var petService = result.Item1[0];

            Assert.AreEqual(petServices[0].Id, petService.Id);
            Assert.AreEqual(petServices[0].ServiceName, petService.Name);
            Assert.AreEqual(petServices[0].Description, petService.Description);
            Assert.AreEqual(petServices[0].Price, petService.Price);
        }

        [Test]
        public async Task AddNewPetServiceTest()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();

            _petServiceRepo.Setup(p => p.AddPetService(It.IsAny<PetServices>())).ReturnsAsync((short)1);

            await _petServiceManagementService.AddNewPetService(petService);

            _petServiceRepo.Verify(p => p.AddPetService(It.IsAny<PetServices>()), Times.Once);
        }

        [Test]
        public async Task UpdatePetServiceTest()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();

            _petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            _petServiceRepo.Setup(p => p.UpdatePetService(It.IsAny<PetServices>())).Returns(Task.CompletedTask);

            await _petServiceManagementService.UpdatePetService(petService);

            _petServiceRepo.Verify(p => p.UpdatePetService(It.IsAny<PetServices>()), Times.Once);
        }
    }
}
