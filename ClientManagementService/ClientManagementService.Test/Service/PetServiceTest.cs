using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineStatus = ClientManagementService.Domain.Models.VaccineStatus;
using RofShared.Exceptions;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class PetServiceTest
    {
        private Mock<IPetRepository> _petRepository;
        private Mock<IPetRetrievalRepository> _petRetrievalRepo;
        private Mock<IPetToVaccinesRepository> _petToVaccinesRepository;

        [SetUp]
        public void Setup()
        {
            _petRepository = new Mock<IPetRepository>();
            _petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            _petToVaccinesRepository = new Mock<IPetToVaccinesRepository>();
        }

        [Test]
        public void AddPet_InvalidInput()
        {
            var newPet = new Domain.Models.Pet()
            {
                Name = "",
                OwnerId = 0,
                BreedId = 0,
                Dob = "",
                Weight = 0
            };

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.AddPet(newPet));
        }

        [Test]
        public void AddPet_AlreadyExist()
        {
            var newPet = new Domain.Models.Pet()
            {
                Name = "Pet1",
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Dob = "1/1/2022",
                Weight = 30,
                Vaccines = new List<VaccineStatus>()
                {
                    new VaccineStatus()
                    {
                        Id = 1,
                        VaxName = "Bordetella",
                        PetToVaccineId = 1,
                        Inoculated = true
                    }
                }
            };

            _petRetrievalRepo.Setup(p => p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<short>()))
                .ReturnsAsync(true);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.AddPet(newPet));
        }

        [Test]
        public async Task AddPet_Success()
        {
            var newPet = new Domain.Models.Pet()
            {
                Name = "Pet1",
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Dob = "1/1/2022",
                Weight = 30,
                Vaccines =new List<VaccineStatus>()
                {
                    new VaccineStatus()
                    {
                        Id = 1,
                        VaxName = "Bordetella",
                        PetToVaccineId = 1,
                        Inoculated = true
                    }
                }
            };

            _petRepository.Setup(p => p.AddPet(It.IsAny<Pet>())).ReturnsAsync(1);
            _petToVaccinesRepository.Setup(p => p.AddPetToVaccines(It.IsAny<List<PetToVaccine>>())).Returns(Task.CompletedTask);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            await petService.AddPet(newPet);

            _petRepository.Verify(p => p.AddPet(It.IsAny<Pet>()), Times.Once);
            _petToVaccinesRepository.Verify(v => v.AddPetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public void UpdatePet_PetNotUnique()
        {
            var pet = new Domain.Models.Pet()
            {
                Id = 1,
                Name = "Pet1",
                OwnerId = 1,                
                BreedId = 1,
                Dob = "1/1/2022",
                Weight = 30,             
                OtherInfo = "",
                Vaccines = new List<VaccineStatus>()
                {
                    new VaccineStatus()
                    {
                        Id = 1,
                        VaxName = "Bordetella",
                        PetToVaccineId = 1,
                        Inoculated = true
                    }
                }
            };

            _petRetrievalRepo.Setup(p => p.DoesPetWithNameAndBreedExistUnderOwner(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<short>()))
               .ReturnsAsync(true);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.UpdatePet(pet));
        }

        [Test]
        public async Task UpdatePet_Success()
        {
            var pet = new Domain.Models.Pet()
            {
                Id = 1,
                Name = "Pet1",
                OwnerId = 1,
                BreedId = 1,
                Dob = "1/1/2022",
                Weight = 30,              
                OtherInfo = "",
                Vaccines = new List<VaccineStatus>()
                {
                    new VaccineStatus()
                    {
                        Id = 1,
                        VaxName = "Bordetella",
                        PetToVaccineId = 1,
                        Inoculated = true
                    }
                }
            };

            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet() { Id = 1 });

            _petRepository.Setup(p => p.UpdatePet(It.IsAny<Pet>())).Returns(Task.CompletedTask);
            _petToVaccinesRepository.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>()
                {
                    new PetToVaccine()
                    {
                        Id = 1,
                        Inoculated = false
                    }
                });
            _petToVaccinesRepository.Setup(v => v.UpdatePetToVaccines(It.IsAny<List<PetToVaccine>>())).Returns(Task.CompletedTask);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            await petService.UpdatePet(pet);

            _petRepository.Verify(p => p.UpdatePet(It.IsAny<Pet>()), Times.Once);
            _petToVaccinesRepository.Verify(v => v.UpdatePetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public void DeletePettById_NoPet()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.DeletePetById(1));
        }

        [Test]
        public async Task DeletePettById_Success()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            await petService.DeletePetById(1);

            _petRepository.Verify(p => p.DeletePetById(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task GetVaccinesByPetId_Success()
        {
            var petToVax = new List<PetToVaccine>()
            {
                new PetToVaccine()
                {
                    Id = 1,
                    PetId = 1,
                    VaxId = 1,
                    Inoculated = true,
                    Vax = new Vaccine() {
                        Id = 1,
                        VaxName = "Bordetella"
                    }
                }
            };

            _petToVaccinesRepository.Setup(pv => pv.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(petToVax);

            var petService = new PetUpsertService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var result = await petService.GetVaccinesByPetId(1);

            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].PetToVaccineId);
            Assert.AreEqual("Bordetella", result[0].VaxName);
            Assert.IsTrue(result[0].Inoculated);
        }
    }
}
