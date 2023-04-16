using ClientManagementService.Domain.Models;
using ClientManagementService.DTO;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class PetControllerTest : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/Pet";

        private readonly string _exceptionMsg = "Test Exception Message";

        [Test]
        public async Task AddPet_Success()
        {
            var newPet = PetCreator.GetPetDTO();

            _petService.Setup(p => p.AddPet(It.IsAny<Domain.Models.Pet>()))
                .ReturnsAsync(1);

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newPet));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task AddPet_BadRequestError()
        {
            var newPet = new PetDTO()
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

            _petService.Setup(p => p.AddPet(It.IsAny<Pet>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newPet));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task AddPet_InternalServerError()
        {
            var newPet = new PetDTO()
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

            _petService.Setup(p => p.AddPet(It.IsAny<Pet>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newPet));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetAllPets_Success()
        {
            var newPets = new List<Pet>()
            {
                PetCreator.GetDomainPet()
            };

            _petService.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new PetsWithTotalPage(newPets, 1));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetAllPets_InternalServerError()
        {
            _petService.Setup(p => p.GetAllPetsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetPetById_Success()
        {
            _petService.Setup(p => p.GetPetById(It.IsAny<long>()))
                .ReturnsAsync(PetCreator.GetDomainPet());

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetPetById_NotFound()
        {
            _petService.Setup(p => p.GetPetById(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Pet"));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _petNotFoundMessage);
        }

        [Test]
        public async Task GetPetById_InternalServerError()
        {
            _petService.Setup(p => p.GetPetById(It.IsAny<long>()))
                .ReturnsAsync((Pet)null);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetPetByName_Success()
        {
            _petService.Setup(p => p.GetPetByName(It.IsAny<string>()))
                .ReturnsAsync(PetCreator.GetDomainPet());

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/Layla/name");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetPetByName_NotFound()
        {
            _petService.Setup(p => p.GetPetByName(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Pet"));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/Layla/name");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _petNotFoundMessage);
        }

        [Test]
        public async Task GetPetByName_InternalServerError()
        {
            _petService.Setup(p => p.GetPetByName(It.IsAny<string>()))
                .ReturnsAsync((Pet)null);

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/Layla/name");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetPetsByClientId_Success()
        {
            var pets = new List<Pet>()
            {
                PetCreator.GetDomainPet()
            };

            _petService.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new PetsWithTotalPage(pets, 1));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/clientId?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetPetsByClientId_InternalServerError()
        {
            _petService.Setup(p => p.GetPetsByClientIdAndKeyword(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/clientId?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdatePetInfo_Success()
        {
            var updatePet = PetCreator.GetPetDTO();

            _petService.Setup(p => p.UpdatePet(It.IsAny<Pet>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/updatePet", ConvertObjectToStringContent(updatePet));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdatePetInfo_NotFound()
        {
            var updatePet = PetCreator.GetPetDTO();

            _petService.Setup(p => p.UpdatePet(It.IsAny<Pet>()))
                .ThrowsAsync(new EntityNotFoundException("Pet"));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/updatePet", ConvertObjectToStringContent(updatePet));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _petNotFoundMessage);
        }

        [Test]
        public async Task UpdatePetInfo_BadRequestError()
        {
            var updatePet = PetCreator.GetPetDTO();

            _petService.Setup(p => p.UpdatePet(It.IsAny<Pet>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/updatePet", ConvertObjectToStringContent(updatePet));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdatePetInfo_InternalServerError()
        {
            var updatePet = PetCreator.GetPetDTO();

            _petService.Setup(p => p.UpdatePet(It.IsAny<Pet>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/updatePet", ConvertObjectToStringContent(updatePet));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeletePetById_Success()
        {
            _petService.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Employee", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task DeletePetById_BadRequestError()
        {
            _petService.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Employee", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeletePetById_InternalServerError()
        {
            _petService.Setup(p => p.DeletePetById(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Employee", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetVaccinesByPetId_Success()
        {
            var vaxStats = new List<VaccineStatus>();
            {
                new VaccineStatus()
                {
                    Id = 1,
                    PetToVaccineId = 1,
                    VaxName = "Bordetella",
                    Inoculated = true
                };
            };

            _petService.Setup(p => p.GetVaccinesByPetId(It.IsAny<long>()))
                .ReturnsAsync(vaxStats);

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1/vax");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetVaccinesByPetId_NotFound()
        {
            _petService.Setup(p => p.GetVaccinesByPetId(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Pet's vaccine records"));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1/vax");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, "Pet's vaccine records not found!");
        }

        [Test]
        public async Task GetVaccinesByPetId_InternalServerError()
        {
            _petService.Setup(p => p.GetVaccinesByPetId(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1/vax");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }
    }
}
