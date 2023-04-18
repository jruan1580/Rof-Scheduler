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

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

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

            _petRetrievalRepo.Setup(p => p.DoesPetExistByNameAndOwner(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

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

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            await petService.AddPet(newPet);

            _petRepository.Verify(p => p.AddPet(It.IsAny<Pet>()), Times.Once);
            _petToVaccinesRepository.Verify(v => v.AddPetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public async Task GetAllPets_NoPets()
        {
            _petRetrievalRepo.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Pet>(), 0));

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

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
                    PetTypeId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    OtherInfo = "",
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
                    },
                    PetType = new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                }
            };

            _petRetrievalRepo.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((pets, 1));

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var result = await petService.GetAllPetsByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Pets);
            Assert.AreEqual("Pet1", result.Pets[0].Name);
            Assert.AreEqual(1, result.Pets[0].OwnerId);
            Assert.AreEqual(1, result.Pets[0].BreedId);
            Assert.AreEqual("1/1/2022", result.Pets[0].Dob);
            Assert.AreEqual(30, result.Pets[0].Weight);         
            Assert.IsEmpty(result.Pets[0].OtherInfo);

            Assert.AreEqual(1, result.Pets[0].OwnerId);
            Assert.IsNotNull(result.Pets[0].Owner);
            Assert.AreEqual(1, result.Pets[0].Owner.Id);
            Assert.AreEqual("John", result.Pets[0].Owner.FirstName);
            Assert.AreEqual("Doe", result.Pets[0].Owner.LastName);

            Assert.AreEqual(1, result.Pets[0].BreedId);
            Assert.IsNotNull(result.Pets[0].BreedInfo);
            Assert.AreEqual(1, result.Pets[0].BreedInfo.Id);
            Assert.AreEqual("Corgi", result.Pets[0].BreedInfo.BreedName);

            Assert.AreEqual(1, result.Pets[0].PetTypeId);
            Assert.IsNotNull(result.Pets[0].PetType);
            Assert.AreEqual(1, result.Pets[0].PetType.Id);
            Assert.AreEqual("Dog", result.Pets[0].PetType.PetTypeName);
        }

        [Test]
        public void GetPetById_DoesNotExist()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetById(1));
        }

        [Test]
        public async Task GetPetById_Success()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    PetTypeId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,                  
                    OtherInfo = "",
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
                    },
                    PetType = new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                });

            _petToVaccinesRepository.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>()
                {
                    new PetToVaccine()
                    {
                        Id = 1,
                        VaxId = 1,
                        Inoculated = true,
                        Vax = new Vaccine()
                        {
                            Id = 1,
                            PetTypeId = 1,
                            VaxName = "Bordetella"
                        }
                    }
                });

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var pet = await petService.GetPetById(1);

            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.PetTypeId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Pet1", pet.Name);
            Assert.AreEqual("1/1/2022", pet.Dob);
            Assert.AreEqual(30, pet.Weight);
            Assert.IsEmpty(pet.OtherInfo);

            Assert.IsNotNull(pet.Owner);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);

            Assert.IsNotNull(pet.BreedInfo);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Corgi", pet.BreedInfo.BreedName);

            Assert.IsNotNull(pet.PetType);
            Assert.AreEqual(1, pet.PetType.Id);
            Assert.AreEqual("Dog", pet.PetType.PetTypeName);

            Assert.IsNotNull(pet.Vaccines);
            Assert.AreEqual(1, pet.Vaccines.Count);

            var vax = pet.Vaccines[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetToVaccineId);
            Assert.AreEqual("Bordetella", vax.VaxName);
            Assert.IsTrue(vax.Inoculated);
        }

        [Test]
        public void GetPetById_MissingVaccines()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    PetTypeId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    OtherInfo = "",
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
                    },
                    PetType = new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                });

            _petToVaccinesRepository.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>());

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetById(1));
        }

        [Test]
        public void GetPetByName_DoesNotExist()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetByName("Pet1"));
        }

        [Test]
        public async Task GetPetByName_Success()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                 .ReturnsAsync(new Pet()
                 {
                     Id = 1,
                     Name = "Pet1",
                     OwnerId = 1,
                     PetTypeId = 1,
                     BreedId = 1,
                     Dob = "1/1/2022",
                     Weight = 30,
                     OtherInfo = "",
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
                     },
                     PetType = new PetType()
                     {
                         Id = 1,
                         PetTypeName = "Dog"
                     }
                 });

            _petToVaccinesRepository.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>()
                {
                    new PetToVaccine()
                    {
                        Id = 1,
                        VaxId = 1,
                        Inoculated = true,
                        Vax = new Vaccine()
                        {
                            Id = 1,
                            PetTypeId = 1,
                            VaxName = "Bordetella"
                        }
                    }
                });

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var pet = await petService.GetPetByName("Pet1");

            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.PetTypeId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Pet1", pet.Name);
            Assert.AreEqual("1/1/2022", pet.Dob);
            Assert.AreEqual(30, pet.Weight);
            Assert.IsEmpty(pet.OtherInfo);

            Assert.IsNotNull(pet.Owner);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);

            Assert.IsNotNull(pet.BreedInfo);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Corgi", pet.BreedInfo.BreedName);

            Assert.IsNotNull(pet.PetType);
            Assert.AreEqual(1, pet.PetType.Id);
            Assert.AreEqual("Dog", pet.PetType.PetTypeName);

            Assert.IsNotNull(pet.Vaccines);
            Assert.AreEqual(1, pet.Vaccines.Count);

            var vax = pet.Vaccines[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetToVaccineId);
            Assert.AreEqual("Bordetella", vax.VaxName);
            Assert.IsTrue(vax.Inoculated);
        }

        [Test]
        public void GetPetByName_MissingVaccines()
        {
            _petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    PetTypeId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    OtherInfo = "",
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
                    },
                    PetType = new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                });

            _petToVaccinesRepository.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>());

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetByName("Pet1"));
        }

        [Test]
        public async Task GetPetsByClientId_NoPets()
        {
            _petRetrievalRepo.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Pet>(), 0));

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var result = await petService.GetPetsByClientIdAndKeyword(1, 1, 10, "");

            Assert.IsEmpty(result.Pets);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetPetsByClientId_Success()
        {
            var pets = new List<Pet>()
            {
                new Pet()
                {
                    Id = 1,
                    Name = "Pet1",
                    OwnerId = 1,
                    PetTypeId = 1,
                    BreedId = 1,
                    Dob = "1/1/2022",
                    Weight = 30,
                    OtherInfo = "",
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
                    },
                    PetType = new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                }
            };

            _petRetrievalRepo.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((pets, 1));

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var result = await petService.GetPetsByClientIdAndKeyword(1, 1, 10, "");

            Assert.IsNotEmpty(result.Pets);

            var pet = result.Pets[0];

            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.PetTypeId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Pet1", pet.Name);
            Assert.AreEqual("1/1/2022", pet.Dob);
            Assert.AreEqual(30, pet.Weight);
            Assert.IsEmpty(pet.OtherInfo);

            Assert.IsNotNull(pet.Owner);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);

            Assert.IsNotNull(pet.BreedInfo);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Corgi", pet.BreedInfo.BreedName);

            Assert.IsNotNull(pet.PetType);
            Assert.AreEqual(1, pet.PetType.Id);
            Assert.AreEqual("Dog", pet.PetType.PetTypeName);
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

            _petRetrievalRepo.Setup(p => p.DoesPetExistByNameAndOwner(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
               .ReturnsAsync(true);

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

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

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            await petService.UpdatePet(pet);

            _petRepository.Verify(p => p.UpdatePet(It.IsAny<Pet>()), Times.Once);
            _petToVaccinesRepository.Verify(v => v.UpdatePetToVaccines(It.IsAny<List<PetToVaccine>>()), Times.Once);
        }

        [Test]
        public void DeletePettById_NoPet()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => petService.DeletePetById(1));
        }

        [Test]
        public async Task DeletePettById_Success()
        {
            _petRepository.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

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

            var petService = new PetService(_petRepository.Object, _petRetrievalRepo.Object, _petToVaccinesRepository.Object);

            var result = await petService.GetVaccinesByPetId(1);

            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].PetToVaccineId);
            Assert.AreEqual("Bordetella", result[0].VaxName);
            Assert.IsTrue(result[0].Inoculated);
        }
    }
}
