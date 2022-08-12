using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class DropdownServiceTest
    {
        private Mock<IPetRepository> _petRepository;
        private Mock<IPetToVaccinesRepository> _petToVaccinesRepository;
        private Mock<IClientRepository> _clientRepository;

        private DropdownService _dropdownService;

        [SetUp]
        public void Setup()
        {
            _petRepository = new Mock<IPetRepository>();

            _petRepository.Setup(p => p.GetAllPetTypes())
                .ReturnsAsync(new List<PetType>()
                {
                    new PetType()
                    {
                        Id = 1,
                        PetTypeName = "Dog"
                    },
                    new PetType()
                    {
                        Id = 2,
                        PetTypeName = "Cat"
                    }
                });

            _petRepository.Setup(p => p.GetPetBreedByPetTypeId(It.IsAny<short>()))
                .ReturnsAsync(new List<Breed>()
                {
                    new Breed()
                    {
                        Id = 1,
                        BreedName = "Golden Retriever"
                    }
                });

            _petToVaccinesRepository = new Mock<IPetToVaccinesRepository>();

            _petToVaccinesRepository.Setup(v => v.GetVaccinesByPetType(It.IsAny<short>()))
                .ReturnsAsync(new List<Vaccine>()
                {
                    new Vaccine()
                    {
                        Id = 1,
                        PetTypeId = 1,
                        VaxName = "Bordetella"
                    }
                });

            _clientRepository = new Mock<IClientRepository>();

            _clientRepository.Setup(c => c.GetClientsForDropdown())
                .ReturnsAsync(new List<Client>()
                {
                    new Client()
                    {
                        Id = 1,
                        FirstName = "Test",
                        LastName = "User",
                    }
                });

            _dropdownService = new DropdownService(_clientRepository.Object, _petRepository.Object, _petToVaccinesRepository.Object);
        }

        [Test]
        public async Task GetVaccinesByPetTypeSuccess()
        {
            var vaccines = await _dropdownService.GetVaccinesByPetType(1);

            Assert.IsNotNull(vaccines);
            Assert.AreEqual(1, vaccines.Count);

            var vax = vaccines[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual("Bordetella", vax.VaxName);
        }

        [Test]
        public async Task GetPetTypeSuccess()
        {
            var petTypes = await _dropdownService.GetPetTypes();

            Assert.IsNotNull(petTypes);
            Assert.AreEqual(2, petTypes.Count);

            var dog = petTypes[0];
            Assert.AreEqual(1, dog.Id);
            Assert.AreEqual("Dog", dog.PetTypeName);

            var cat = petTypes[1];
            Assert.AreEqual(2, cat.Id);
            Assert.AreEqual("Cat", cat.PetTypeName);
        }

        [Test]
        public async Task GetPetBreedByPetTypeSuccess()
        {
            var breeds = await _dropdownService.GetBreedsByPetType(1);

            Assert.IsNotNull(breeds);
            Assert.AreEqual(1, breeds.Count);

            var breed = breeds[0];
            Assert.AreEqual(1, breed.Id);
            Assert.AreEqual("Golden Retriever", breed.BreedName);
        }

        [Test]
        public async Task GetClientsSuccess()
        {
            var clients = await _dropdownService.GetClients();

            Assert.IsNotNull(clients);
            Assert.AreEqual(1, clients.Count);

            var client = clients[0];
            Assert.AreEqual(1, client.Id);
            Assert.AreEqual("Test", client.FirstName);
            Assert.AreEqual("User", client.LastName);
            Assert.AreEqual("Test User", client.FullName);
        }
    }
}
