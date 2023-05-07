using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineStatus = ClientManagementService.Domain.Models.VaccineStatus;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class PetUpsertServiceTest
    {
        [Test]
        public void AddPet_InvalidPet()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var newPet = new Domain.Models.Pet()
            {
                Name = "",
                OwnerId = 0,
                BreedId = 0,
                Dob = "",
                Weight = 0
            };

            var petService = new PetUpsertService(petRetrievalRepo.Object, 
                petUpsertRepo.Object, 
                petToVaccineRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.AddPet(newPet));
        }

        [Test]
        public void AddPet_NotUniqueToClient()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var newPet = PetCreator.GetDomainPet();

            petRetrievalRepo.Setup(p => p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(), 
                It.IsAny<long>(), 
                It.IsAny<string>(), 
                It.IsAny<short>()))
            .ReturnsAsync(true);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.AddPet(newPet));
        }

        [Test]
        public async Task AddPet_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var newPet = PetCreator.GetDomainPet();
            newPet.Vaccines = new List<VaccineStatus>()
            {
                VaccineCreator.GetDomainVaccine()
            };

            petRetrievalRepo.Setup(p =>
                p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<short>()))
            .ReturnsAsync(false);

            petUpsertRepo.Setup(p => p.AddPet(It.IsAny<Pet>())).ReturnsAsync(1);
            petToVaccineRepo.Setup(p => p.AddPetToVaccines(It.IsAny<List<PetToVaccine>>())).Returns(Task.CompletedTask);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            await petService.AddPet(newPet);

            petUpsertRepo.Verify(p => p.AddPet(It.Is<Pet>(p => p.Id == newPet.Id)), Times.Once);
            petToVaccineRepo.Verify(v => v.AddPetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public void UpdatePet_InvalidPet()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var updatePet = new Domain.Models.Pet()
            {
                Name = "",
                OwnerId = 0,
                BreedId = 0,
                Dob = "",
                Weight = 0
            };

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.UpdatePet(updatePet));
        }

        [Test]
        public void UpdatePet_NotUniqueToClient()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var updatePet = PetCreator.GetDomainPet();

            petRetrievalRepo.Setup(p => 
                p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(), 
                    It.IsAny<long>(), 
                    It.IsAny<string>(), 
                    It.IsAny<short>()))
            .ReturnsAsync(true);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.UpdatePet(updatePet));
        }

        [Test]
        public async Task UpdatePet_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var updatePet = PetCreator.GetDomainPet();
            updatePet.Vaccines = new List<VaccineStatus>()
            {
                VaccineCreator.GetDomainVaccine()
            };

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(PetCreator.GetDbPet());

            petRetrievalRepo.Setup(p =>
                p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<short>()))
            .ReturnsAsync(false);

            petUpsertRepo.Setup(p => p.UpdatePet(It.IsAny<Pet>())).Returns(Task.CompletedTask);

            petToVaccineRepo.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>()
                {
                    VaccineCreator.GetDbPetToVaccine()
                });

            petToVaccineRepo.Setup(v => v.UpdatePetToVaccines(It.IsAny<List<PetToVaccine>>())).Returns(Task.CompletedTask);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            await petService.UpdatePet(updatePet);

            petUpsertRepo.Verify(p => p.UpdatePet(It.Is<Pet>(p => p.Id == updatePet.Id)), Times.Once);
            petToVaccineRepo.Verify(v => v.UpdatePetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public void DeletePetById_NotFound()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => 
                p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
            .ReturnsAsync((Pet)null);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            petUpsertRepo.Verify(c => c.DeletePetById(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task DeletePetById_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petUpsertRepo = new Mock<IPetUpsertRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
               .ReturnsAsync(PetCreator.GetDbPet());

            petUpsertRepo.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var petService = new PetUpsertService(petRetrievalRepo.Object,
                petUpsertRepo.Object,
                petToVaccineRepo.Object);

            await petService.DeletePetById(1);

            petUpsertRepo.Verify(p => p.DeletePetById(It.IsAny<long>()), Times.Once);
        }
    }
}
