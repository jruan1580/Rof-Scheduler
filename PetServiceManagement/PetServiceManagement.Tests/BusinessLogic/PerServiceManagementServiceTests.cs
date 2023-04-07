using Moq;
using NUnit.Framework;
using PetServiceManagement.Domain.BusinessLogic;
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
        [Test]
        public void PetServiceValidationNoNameTest()
        {
            var petServiceManagementService = new PetServiceManagementService(null, null);

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(null));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(null));

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Name = string.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidPrice()
        {
            var petServiceManagementService = new PetServiceManagementService(null, null);

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Price = -1m;

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidEmployeeRate()
        {
            var petServiceManagementService = new PetServiceManagementService(null, null);

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.EmployeeRate = -1m;

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));

            petService.EmployeeRate = 101m;

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidDuration()
        {
            var petServiceManagementService = new PetServiceManagementService(null, null);

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.Duration = -1;

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void PetServiceValidationInvalidTimeUnit()
        {
            var petServiceManagementService = new PetServiceManagementService(null, null);

            var petService = PetServiceFactory.GetPetServiceDomain();
            petService.TimeUnit = "Sec";

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.AddNewPetService(petService));
            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public void UnableToFindForUpdateTest()
        {
            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();

            petServiceRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync((PetServices)null);

            var petServiceManagementService = new PetServiceManagementService(petServiceRepo.Object, null);

            var petService = PetServiceFactory.GetPetServiceDomain();

            Assert.ThrowsAsync<ArgumentException>(() => petServiceManagementService.UpdatePetService(petService));
        }

        [Test]
        public async Task GetByPageSuccessTest()
        {
            var petServices = new List<PetServices>()
            {
                PetServiceFactory.GetPetServicesDbEntity(),
            };

            var petServiceRepo = new Mock<IPetServiceRetrievalRepository>();

            petServiceRepo.Setup(p => p.GetAllPetServicesByPageAndKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((petServices, 1));

            var petServiceManagementService = new PetServiceManagementService(petServiceRepo.Object, null);

            var result = await petServiceManagementService.GetPetServicesByPageAndKeyword(1, 1, "Walking");
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

            var petServiceRepo = new Mock<IPetServiceUpsertRepository>();

            petServiceRepo.Setup(p => p.AddPetService(It.IsAny<PetServices>())).ReturnsAsync((short)1);

            var petServiceManagementService = new PetServiceManagementService(null, petServiceRepo.Object);

            await petServiceManagementService.AddNewPetService(petService);

            petServiceRepo.Verify(p => p.AddPetService(It.IsAny<PetServices>()), Times.Once);
        }

        [Test]
        public async Task UpdatePetServiceTest()
        {
            var petService = PetServiceFactory.GetPetServiceDomain();

            var petServiceRetrievalRepo = new Mock<IPetServiceRetrievalRepository>();
            var petServiceUpsertRepo = new Mock<IPetServiceUpsertRepository>();

            petServiceRetrievalRepo.Setup(p => p.GetPetServiceById(It.IsAny<short>()))
                .ReturnsAsync(PetServiceFactory.GetPetServicesDbEntity());

            petServiceUpsertRepo.Setup(p => p.UpdatePetService(It.IsAny<PetServices>())).Returns(Task.CompletedTask);

            var petServiceManagementService = new PetServiceManagementService(petServiceRetrievalRepo.Object, petServiceUpsertRepo.Object);

            await petServiceManagementService.UpdatePetService(petService);

            petServiceUpsertRepo.Verify(p => p.UpdatePetService(It.IsAny<PetServices>()), Times.Once);
        }
    }
}
