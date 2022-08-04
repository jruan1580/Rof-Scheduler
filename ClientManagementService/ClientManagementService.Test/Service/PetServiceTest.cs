using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
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
                Weight = 30,
                Owner = new Domain.Models.Client()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",

                },
                BreedInfo = new Domain.Models.Breed()
                {
                    Id = 1,
                    BreedName = "Corgi"            
                }
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
                    Picture = null,
                    Owner = new Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",

                    },
                    Breed = new Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"            
                    }
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
            Assert.AreEqual(1, result.Pets[0].Owner.Id);
            Assert.AreEqual("John", result.Pets[0].Owner.FirstName);
            Assert.AreEqual("Doe", result.Pets[0].Owner.LastName);
            Assert.AreEqual(1, result.Pets[0].BreedInfo.Id);
            Assert.AreEqual("Corgi", result.Pets[0].BreedInfo.BreedName);
        }

        [Test]
        public void GetPetById_DoesNotExist()
        {
            _petRepository.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetService(_petRepository.Object);

            Assert.ThrowsAsync<PetNotFoundException>(() => petService.GetPetById(1));
        }

        [Test]
        public async Task GetPetById_Success()
        {
            _petRepository.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    Dhppvax = true,
                    BordetellaVax = true,
                    RabieVax = true,
                    OtherInfo = "",
                    Picture = null,
                    Owner = new Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",

                    },
                    Breed = new Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    }
                });

            var petService = new PetService(_petRepository.Object);

            var pet = await petService.GetPetById(1);

            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Pet1", pet.Name);
            Assert.AreEqual("1/1/2022", pet.Dob);
            Assert.AreEqual(30, pet.Weight);
            Assert.IsTrue(pet.Dhppvax);
            Assert.IsTrue(pet.BordetellaVax);
            Assert.IsTrue(pet.RabieVax);
            Assert.IsEmpty(pet.OtherInfo);
            Assert.IsNull(pet.Picture);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Corgi", pet.BreedInfo.BreedName);
        }

        [Test]
        public void GetPetByName_DoesNotExist()
        {
            _petRepository.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetService(_petRepository.Object);

            Assert.ThrowsAsync<PetNotFoundException>(() => petService.GetPetByName("Pet1"));
        }

        [Test]
        public async Task GetPetByName_Success()
        {
            _petRepository.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                .ReturnsAsync(new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    Dhppvax = true,
                    BordetellaVax = true,
                    RabieVax = true,
                    OtherInfo = "",
                    Picture = null,
                    Owner = new Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",

                    },
                    Breed = new Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    }
                });

            var petService = new PetService(_petRepository.Object);

            var pet = await petService.GetPetByName("Pet1");

            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Pet1", pet.Name);
            Assert.AreEqual("1/1/2022", pet.Dob);
            Assert.AreEqual(30, pet.Weight);
            Assert.IsTrue(pet.Dhppvax);
            Assert.IsTrue(pet.BordetellaVax);
            Assert.IsTrue(pet.RabieVax);
            Assert.IsEmpty(pet.OtherInfo);
            Assert.IsNull(pet.Picture);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Corgi", pet.BreedInfo.BreedName);
        }

        [Test]
        public async Task GetPetsByClientId_NoPets()
        {
            _petRepository.Setup(p => p.GetPetsByClientId(It.IsAny<long>()))
                .ReturnsAsync(new List<Pet>());

            var petService = new PetService(_petRepository.Object);

            var result = await petService.GetPetsByClientId(1);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetPetsByClientId_Success()
        {
            _petRepository.Setup(p => p.GetPetsByClientId(It.IsAny<long>()))
                .ReturnsAsync(new List<Pet>());

            var petService = new PetService(_petRepository.Object);

            var result = await petService.GetPetsByClientId(1);

            Assert.IsEmpty(result);
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
                Dhppvax = true,
                BordetellaVax = true,
                RabieVax = true,
                OtherInfo = "",
                Picture = null
            };

            _petRepository.Setup(p => p.PetAlreadyExists(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
               .ReturnsAsync(true);

            var petService = new PetService(_petRepository.Object);

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
                Dhppvax = true,
                BordetellaVax = true,
                RabieVax = true,
                OtherInfo = "",
                Picture = null
            };

            _petRepository.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet() { Id = 1 });

            var petService = new PetService(_petRepository.Object);

            await petService.UpdatePet(pet);

            _petRepository.Verify(p => p.UpdatePet(It.IsAny<Pet>()), Times.Once);
        }

        [Test]
        public void DeletePettById_NoPet()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var petService = new PetService(_petRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.DeletePetById(1));
        }

        [Test]
        public async Task DeletePettById_Success()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var petService = new PetService(_petRepository.Object);

            await petService.DeletePetById(1);

            _petRepository.Verify(p => p.DeletePetById(It.IsAny<long>()), Times.Once);
        }
    }
}
