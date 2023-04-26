using ClientManagementService.Domain.Services;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using DbPetType = ClientManagementService.Infrastructure.Persistence.Entities.PetType;
using DbVaccine = ClientManagementService.Infrastructure.Persistence.Entities.Vaccine;
using DbBreed = ClientManagementService.Infrastructure.Persistence.Entities.Breed;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class DropdownServiceTest
    {
        [Test]
        public async Task GetVaccinesByPetTypeSuccess()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var vaccines = new List<DbVaccine>()
            {
                DropdownCreator.GetDbVaccineForDropdown()
            };

            petToVaccineRepo.Setup(v => v.GetVaccinesByPetType(It.IsAny<short>()))
                .ReturnsAsync(vaccines);

            var dropdownService = new DropdownService(clientRetrievalRepo.Object, 
                petRetrievalRepo.Object, 
                petToVaccineRepo.Object);

            var results = await dropdownService.GetVaccinesByPetType(1);

            AssertResultsNotNullAndCount<Vaccine>(results);

            var vax = results[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual("Bordetella", vax.VaxName);
        }

        [Test]
        public async Task GetPetTypeSuccess()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var petTypes = new List<DbPetType>()
            {
                DropdownCreator.GetDbPetTypeForDropdown()
            };

            petRetrievalRepo.Setup(p => p.GetPetTypesForDropdown())
                .ReturnsAsync(petTypes);

            var dropdownService = new DropdownService(clientRetrievalRepo.Object,
                petRetrievalRepo.Object,
                petToVaccineRepo.Object);

            var results = await dropdownService.GetPetTypes();

            AssertResultsNotNullAndCount<PetType>(results);

            var dog = results[0];
            Assert.AreEqual(1, dog.Id);
            Assert.AreEqual("Dog", dog.PetTypeName);
        }

        [Test]
        public async Task GetPetBreedByPetTypeSuccess()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var breeds = new List<DbBreed>()
            {
                DropdownCreator.GetDbBreedForDropdown()
            };

            petRetrievalRepo.Setup(p => p.GetBreedsByPetTypeIdForDropdown(It.IsAny<short>()))
                .ReturnsAsync(breeds);

            var dropdownService = new DropdownService(clientRetrievalRepo.Object,
                petRetrievalRepo.Object,
                petToVaccineRepo.Object);

            var results = await dropdownService.GetBreedsByPetType(1);

            AssertResultsNotNullAndCount<Breed>(results);

            var breed = results[0];
            Assert.AreEqual(1, breed.Id);
            Assert.AreEqual("Golden Retriever", breed.BreedName);
        }

        [Test]
        public async Task GetClientsSuccess()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var clients = new List<DbClient>()
            {
                DropdownCreator.GetDbClientForDropdown()
            };

            clientRetrievalRepo.Setup(c => c.GetClientsForDropdown())
                .ReturnsAsync(clients);

            var dropdownService = new DropdownService(clientRetrievalRepo.Object,
                petRetrievalRepo.Object,
                petToVaccineRepo.Object);

            var results = await dropdownService.GetClients();

            AssertResultsNotNullAndCount<Client>(results);

            var client = results[0];
            Assert.AreEqual(1, client.Id);
            Assert.AreEqual("Test", client.FirstName);
            Assert.AreEqual("User", client.LastName);
            Assert.AreEqual("Test User", client.FullName);
        }

        [Test]
        public async Task GetPetsSuccess()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var petRetrievalRepo = new Mock<IPetRetrievalRepository>();
            var petToVaccineRepo = new Mock<IPetToVaccinesRepository>();

            var pets = new List<DbPet>()
            {
                DropdownCreator.GetDbPetForDropdown()
            };

            petRetrievalRepo.Setup(p => p.GetPetsForDropdown())
                .ReturnsAsync(pets);

            var dropdownService = new DropdownService(clientRetrievalRepo.Object,
                petRetrievalRepo.Object,
                petToVaccineRepo.Object);

            var results = await dropdownService.GetPets();

            AssertResultsNotNullAndCount<Pet>(results);

            var pet = results[0];
            Assert.AreEqual(1, pet.Id);
            Assert.AreEqual("Layla", pet.Name);
        }

        private void AssertResultsNotNullAndCount<T>(List<T> results)
        {
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
        }
    }
}
