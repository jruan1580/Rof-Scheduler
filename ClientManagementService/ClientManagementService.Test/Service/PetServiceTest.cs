﻿using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class PetServiceTest
    {
        private Mock<IPetRepository> _petRepository;

        [SetUp]
        public void Setup()
        {
            _petRepository = new Mock<IPetRepository>();
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

            var petService = new PetService(_petRepository.Object);

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
                Dob = "1/1/2022",
                Weight = 30
            };

            _petRepository.Setup(p => p.PetAlreadyExists(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var petService = new PetService(_petRepository.Object);

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
                Dob = "1/1/2022",
                Weight = 30
            };

            var petService = new PetService(_petRepository.Object);

            await petService.AddPet(newPet);

            _petRepository.Verify(p => p.AddPet(It.IsAny<Pet>()), Times.Once);
        }

        [Test]
        public async Task GetAllPets_NoPets()
        {
            _petRepository.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Pet>(), 0));

            var petService = new PetService(_petRepository.Object);

            var result = await petService.GetAllPetsByKeyword(1, 10, "");

            Assert.IsEmpty(result.Pets);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAllPets_Success()
        {
            var pets = new List<Pet>()
            {
                new Pet()
                {
                    Name = "Pet1",
                    OwnerId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    Dhppvax = true,
                    BordetellaVax = true,
                    RabieVax = true,
                    OtherInfo = "",
                    Picture = null
                }
            };

            _petRepository.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((pets, 1));

            var petService = new PetService(_petRepository.Object);

            var result = await petService.GetAllPetsByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Pets);
            Assert.AreEqual("Pet1", result.Pets[0].Name);
            Assert.AreEqual(1, result.Pets[0].OwnerId);
            Assert.AreEqual(1, result.Pets[0].BreedId);
            Assert.AreEqual("1/1/2022", result.Pets[0].Dob);
            Assert.AreEqual(30, result.Pets[0].Weight);
            Assert.IsTrue(result.Pets[0].Dhppvax);
            Assert.IsTrue(result.Pets[0].BordetellaVax);
            Assert.IsTrue(result.Pets[0].RabieVax);
            Assert.IsEmpty(result.Pets[0].OtherInfo);
            Assert.IsNull(result.Pets[0].Picture);
        }
    }
}
