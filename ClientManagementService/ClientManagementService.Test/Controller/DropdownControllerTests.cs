using ClientManagementService.Domain.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class DropdownControllerTests : ApiTestSetup
    {

        private readonly string _baseUrl = "/api/Dropdown";

        [Test]
        public async Task GetClientTest()
        {
            var clients = new List<Client>()
            {
                DropdownCreator.GetDomainClientForDropdown()
            };

            _dropdownService.Setup(d => d.GetClients())
                .ReturnsAsync(clients);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/clients");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetPetTest()
        {
            var pets = new List<Pet>()
            {
                DropdownCreator.GetDomainPetForDropdown()
            };

            _dropdownService.Setup(d => d.GetPets())
                .ReturnsAsync(pets);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/pets");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetPetTypesTest()
        {
            var petTypes = new List<PetType>()
            {
                DropdownCreator.GetDomainPetTypeForDropdown()
            };

            _dropdownService.Setup(d => d.GetPetTypes())
                .ReturnsAsync(petTypes);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/petTypes");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetVaccinesByPetTypeTest()
        {
            var vaccines = new List<Vaccine>()
            {
                DropdownCreator.GetDomainVaccineForDropdown()
            };

            _dropdownService.Setup(d => d.GetVaccinesByPetType(It.IsAny<short>()))
                .ReturnsAsync(vaccines);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1/vaccines");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetBreedByPetTypeTest()
        {
            var breeds = new List<Breed>()
            {
                DropdownCreator.GetDomainBreedForDropdown()
            };

            _dropdownService.Setup(d => d.GetBreedsByPetType(It.IsAny<short>()))
                .ReturnsAsync(breeds);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1/breeds");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }
    }
}
