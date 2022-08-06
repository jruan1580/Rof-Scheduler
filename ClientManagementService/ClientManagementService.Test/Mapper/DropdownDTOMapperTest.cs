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
    }
}
