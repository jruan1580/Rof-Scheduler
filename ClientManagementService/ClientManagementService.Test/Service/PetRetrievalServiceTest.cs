using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class PetRetrievalServiceTest
    {
        [Test]
        public async Task GetAllPets_NoPets()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => p.GetAllPetsByKeyword(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<string>()))
            .ReturnsAsync((new List<Pet>(), 0));

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var result = await petService.GetAllPetsByKeyword(1, 10, "");

            Assert.IsEmpty(result.Pets);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAllPets_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pets = new List<Pet>()
            {
                PetCreator.GetDbPet()
            };

            petRetrievalRepo.Setup(p => p.GetAllPetsByKeyword(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync((pets, 1));

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var result = await petService.GetAllPetsByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Pets);
            AssertPetExpectedEqualsActualValues(result.Pets[0]);
        }

        [Test]
        public void GetPetById_NotFound()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetById(1));
        }

        [Test]
        public async Task GetPetById_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pet = PetCreator.GetDbPet();
            var petToVaccines = new List<PetToVaccine>()
            {
                VaccineCreator.GetDbPetToVaccine()
            };

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(pet);

            petToVaccineRepo.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(petToVaccines);

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var corePet = await petService.GetPetById(1);

            Assert.IsNotNull(corePet);
            AssertPetExpectedEqualsActualValues(corePet);
            AssertPetVaccineExpectedEqualsActualValues(corePet.Vaccines);
        }

        [Test]
        public void GetPetById_MissingVaccines()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pet = PetCreator.GetDbPet();

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(pet);

            petToVaccineRepo.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>());

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetById(1));
        }

        [Test]
        public void GetPetByName_NotFound()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                .ReturnsAsync((Pet)null);

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetByName("Layla"));
        }

        [Test]
        public async Task GetPetByName_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pet = PetCreator.GetDbPet();
            var petToVaccines = new List<PetToVaccine>()
            {
                VaccineCreator.GetDbPetToVaccine()
            };

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<string>>()))
                 .ReturnsAsync(pet);

            petToVaccineRepo.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(petToVaccines);

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var corePet = await petService.GetPetByName("Layla");

            Assert.IsNotNull(corePet);
            AssertPetExpectedEqualsActualValues(corePet);
            AssertPetVaccineExpectedEqualsActualValues(corePet.Vaccines);
        }

        [Test]
        public void GetPetByName_MissingVaccines()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pet = PetCreator.GetDbPet();

            petRetrievalRepo.Setup(p => p.GetPetByFilter(It.IsAny<GetPetFilterModel<long>>()))
                .ReturnsAsync(pet);

            petToVaccineRepo.Setup(v => v.GetPetToVaccineByPetId(It.IsAny<long>()))
                .ReturnsAsync(new List<PetToVaccine>());

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => petService.GetPetByName("Layla"));
        }

        [Test]
        public async Task GetPetsByClientId_NoPets()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            petRetrievalRepo.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Pet>(), 0));

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var result = await petService.GetPetsByClientIdAndKeyword(1, 1, 10, "");

            Assert.IsEmpty(result.Pets);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetPetsByClientId_Success()
        {
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pets = new List<Pet>()
            {
                PetCreator.GetDbPet()
            };

            petRetrievalRepo.Setup(p => p.GetPetsByClientIdAndKeyword(
                It.IsAny<long>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<string>()))
            .ReturnsAsync((pets, 1));

            var petService = new PetRetrievalService(petRetrievalRepo.Object, petToVaccineRepo.Object);

            var result = await petService.GetPetsByClientIdAndKeyword(1, 1, 10, "");

            Assert.IsNotEmpty(result.Pets);
            AssertPetExpectedEqualsActualValues(result.Pets[0]);
        }

        private void AssertPetExpectedEqualsActualValues(Domain.Models.Pet pet)
        {
            Assert.IsNotNull(pet);
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual(1, pet.OwnerId);
            Assert.AreEqual(1, pet.PetTypeId);
            Assert.AreEqual(1, pet.BreedId);
            Assert.AreEqual("Layla", pet.Name);
            Assert.AreEqual("3/14/2019", pet.Dob);
            Assert.AreEqual(70, pet.Weight);

            Assert.IsNotNull(pet.Owner);
            Assert.AreEqual(1, pet.Owner.Id);
            Assert.AreEqual("John", pet.Owner.FirstName);
            Assert.AreEqual("Doe", pet.Owner.LastName);

            Assert.IsNotNull(pet.BreedInfo);
            Assert.AreEqual(1, pet.BreedInfo.Id);
            Assert.AreEqual("Golden Retriever", pet.BreedInfo.BreedName);

            Assert.IsNotNull(pet.PetType);
            Assert.AreEqual(1, pet.PetType.Id);
            Assert.AreEqual("Dog", pet.PetType.PetTypeName);
        }

        private void AssertPetVaccineExpectedEqualsActualValues(List<Domain.Models.VaccineStatus> vax)
        {
            Assert.IsNotNull(vax);
            Assert.AreEqual(1, vax.Count);
            Assert.AreEqual(1, vax[0].Id);
            Assert.AreEqual(1, vax[0].PetToVaccineId);
            Assert.AreEqual("Bordetella", vax[0].VaxName);
            Assert.IsTrue(vax[0].Inoculated);
        }
    }
}
