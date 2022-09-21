using ClientManagementService.API.Controllers;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class PetControllerTest
    {
        private Mock<IPetService> _petService;

        [SetUp]
        public void Setup()
        {
            _petService = new Mock<IPetService>();
        }

        [Test]
        public async Task AddPet_Success()
        {
            var newPet = new API.DTO.PetDTO()
            {
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Name = "Pet1",
                Weight = 30,
                Dob = "1/1/2022",
                OwnerFirstName = "John",
                OwnerLastName = "Doe",
                BreedName = "Corgi",
                Vaccines = new List<API.DTO.PetsVaccineDTO>()
                {
                    new API.DTO.PetsVaccineDTO()
                    {
                        Id = 1,
                        PetsVaccineId = 1,
                        VaccineName = "Bordetella",
                        Inoculated = true
                    }
                }
            };

            _petService.Setup(p => p.AddPet(It.IsAny<Domain.Models.Pet>()))
                .ReturnsAsync(1);

            var controller = new PetController(_petService.Object);

            var response = await controller.AddPet(newPet);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task AddPet_InternalServerError()
        {
            var newPet = new API.DTO.PetDTO()
            {
                OwnerId = 0,
                BreedId = 0,
                Name = "",
                Weight = 0,
                Dob = "",
                OwnerFirstName = "",
                OwnerLastName = "",
                BreedName = ""
            };

            _petService.Setup(p => p.AddPet(It.IsAny<Domain.Models.Pet>()))
                .ThrowsAsync(new Exception());

            var controller = new PetController(_petService.Object);

            var response = await controller.AddPet(newPet);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetAllPets_Success()
        {
            var pets = new List<Domain.Models.Pet>()
            {
                new Domain.Models.Pet()
                {
                    Id = 1,
                    OwnerId = 1,
                    BreedId = 1,
                    PetTypeId = 1,
                    Name = "Pet1",
                    Weight = 30,
                    Dob = "1/1/2022",
                    Owner = new Domain.Models.Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    BreedInfo = new Domain.Models.Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    },
                    PetType = new Domain.Models.PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                }
            };

            _petService.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.PetsWithTotalPage(pets, 1));

            var controller = new PetController(_petService.Object);

            var response = await controller.GetAllPets(1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetAllPets_InternalServerError()
        {
            _petService.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new PetController(_petService.Object);

            var response = await controller.GetAllPets(1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetPetById_Success()
        {
            _petService.Setup(p => p.GetPetById(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Models.Pet()
                {
                    Id = 1,
                    OwnerId = 1,
                    BreedId = 1,
                    PetTypeId = 1,
                    Name = "Pet1",
                    Weight = 30,
                    Dob = "1/1/2022",
                    Owner = new Domain.Models.Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    BreedInfo = new Domain.Models.Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    },
                    PetType = new Domain.Models.PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    },
                    Vaccines = new List<Domain.Models.VaccineStatus>()
                    {
                        new Domain.Models.VaccineStatus()
                        {
                            Id = 1,
                            PetToVaccineId = 1,
                            VaxName = "Bordetella",
                            Inoculated = true
                        }
                    }
                });

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetPetById_InternalServerError()
        {
            _petService.Setup(p => p.GetPetById(It.IsAny<long>()))
                .ReturnsAsync((Domain.Models.Pet)null);

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetPetByName_Success()
        {
            _petService.Setup(p => p.GetPetByName(It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.Pet()
                {
                    Id = 1,
                    OwnerId = 1,
                    BreedId = 1,
                    PetTypeId = 1,
                    Name = "Pet1",
                    Weight = 30,
                    Dob = "1/1/2022",
                    Owner = new Domain.Models.Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    BreedInfo = new Domain.Models.Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    },
                    PetType = new Domain.Models.PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    },
                    Vaccines = new List<Domain.Models.VaccineStatus>()
                    {
                        new Domain.Models.VaccineStatus()
                        {
                            Id = 1,
                            PetToVaccineId = 1,
                            VaxName = "Bordetella",
                            Inoculated = true
                        }
                    }
                });

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetByName("Pet1");

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetPetByName_InternalServerError()
        {
            _petService.Setup(p => p.GetPetByName(It.IsAny<string>()))
                .ReturnsAsync((Domain.Models.Pet)null);

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetByName("Pet1");

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetPetsByClientId_Success()
        {
            var pets = new List<Domain.Models.Pet>()
            {
                new Domain.Models.Pet()
                {
                    Id = 1,
                    OwnerId = 1,
                    BreedId = 1,
                    PetTypeId = 1,
                    Name = "Pet1",
                    Weight = 30,
                    Dob = "1/1/2022",
                    Owner = new Domain.Models.Client()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    BreedInfo = new Domain.Models.Breed()
                    {
                        Id = 1,
                        BreedName = "Corgi"
                    },
                    PetType = new Domain.Models.PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    }
                }
            };

            _petService.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.PetsWithTotalPage(pets, 1));

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetsByClientId(1, 1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetPetsByClientId_InternalServerError()
        {
            _petService.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new PetController(_petService.Object);

            var response = await controller.GetPetsByClientId(1, 1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task UpdatePet_Success()
        {
            var pet = new API.DTO.PetDTO()
            {
                Id = 1,
                OwnerId = 1,
                BreedId = 1,
                PetTypeId = 1,
                Name = "Pet1",
                Weight = 30,
                Dob = "1/1/2022",
                OwnerFirstName = "John",
                OwnerLastName = "Doe",
                BreedName = "Corgi",
                Vaccines = new List<API.DTO.PetsVaccineDTO>()
                {
                    new API.DTO.PetsVaccineDTO()
                    {
                        Id = 1,
                        PetsVaccineId = 1,
                        VaccineName = "Bordetella",
                        Inoculated = true
                    }
                }
            };

            _petService.Setup(p => p.UpdatePet(It.IsAny<Domain.Models.Pet>()))
                .Returns(Task.CompletedTask);

            var controller = new PetController(_petService.Object);

            var response = await controller.UpdatePetInfo(pet);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdatePetInfo_BadRequestError()
        {
            var pet = new API.DTO.PetDTO()
            {
                Id = 0,
                OwnerId = 1,
                BreedId = 1,
                Name = "Pet1",
                Weight = 30,
                Dob = "1/1/2022",
                OwnerFirstName = "John",
                OwnerLastName = "Doe",
                BreedName = "Corgi"
            };

            _petService.Setup(p => p.UpdatePet(It.IsAny<Domain.Models.Pet>()))
                .ThrowsAsync(new ArgumentException());

            var controller = new PetController(_petService.Object);

            var response = await controller.UpdatePetInfo(pet);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task DeletePetById_Success()
        {
            _petService.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var controller = new PetController(_petService.Object);

            var response = await controller.DeletePetById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task DeletePetById_BadRequestError()
        {
            _petService.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var controller = new PetController(_petService.Object);

            var response = await controller.DeletePetById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task GetVaccinesByPetId_Success()
        {
            var vaxStats = new List<Domain.Models.VaccineStatus>();
            {
                new Domain.Models.VaccineStatus()
                {
                    Id = 1,
                    PetToVaccineId = 1,
                    VaxName = "Bordetella",
                    Inoculated = true
                };
            };

            _petService.Setup(p => p.GetVaccinesByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Models.VaccineStatus>());

            var controller = new PetController(_petService.Object);

            var response = await controller.GetVaccinesByPetId(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetVaccinesByPetId_InternalServerError()
        {
            _petService.Setup(p => p.GetVaccinesByPetId(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            var controller = new PetController(_petService.Object);

            var response = await controller.GetVaccinesByPetId(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }
    }
}
