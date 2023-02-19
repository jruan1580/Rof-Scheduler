using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class DropdownDTOMapperTest
    {
        [Test]
        public void TestToVaccineDTO()
        {
            var vaccine = new Vaccine()
            {
                Id = 1,
                VaxName = "Bordetella"
            };

            var dtos = DropdownDTOMapper.ToVaccineDTO(new List<Vaccine> { vaccine });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var vaccineDto = dtos[0];

            Assert.AreEqual(1, vaccineDto.Id);
            Assert.AreEqual("Bordetella", vaccineDto.VaccineName);
        }

        [Test]
        public void TestToPetTypeDTO()
        {
            var petType = new PetType()
            {
                Id = 1,
                PetTypeName = "Dog"
            };

            var dtos = DropdownDTOMapper.ToPetTypeDTO(new List<PetType> { petType });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var petTypeDTO = dtos[0];

            Assert.AreEqual(1, petTypeDTO.Id);
            Assert.AreEqual("Dog", petTypeDTO.PetTypeName);
        }

        [Test]
        public void TestToBreedDTO()
        {
            var breed = new Breed()
            {
                Id = 1,
                BreedName = "Golden Retriever"
            };

            var dtos = DropdownDTOMapper.ToBreedDTO(new List<Breed> { breed });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var dto = dtos[0];
            Assert.AreEqual(dto.Id, breed.Id);
            Assert.AreEqual(dto.BreedName, breed.BreedName);
        }

        [Test]
        public void TestToClientDTO()
        {
            var client = new Client()
            {
                Id = 1,
                FullName = "Test User"
            };

            var dtos = DropdownDTOMapper.ToClientDTO(new List<Client> { client });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var dto = dtos[0];
            Assert.AreEqual(dto.Id, client.Id);
            Assert.AreEqual(dto.FullName, client.FullName);
        }

        [Test]
        public void TestToPetDTO()
        {
            var pet = new Pet()
            {
                Id = 1,
                Name = "TestPet"
            };

            var dtos = DropdownDTOMapper.ToPetDTO(new List<Pet> { pet });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var dto = dtos[0];
            Assert.AreEqual(dto.Id, pet.Id);
            Assert.AreEqual(dto.Name, pet.Name);
        }
    }
}
