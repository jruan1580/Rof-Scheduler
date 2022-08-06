using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using CorePet = ClientManagementService.Domain.Models.Pet;
using DTOPet = ClientManagementService.API.DTO.PetDTO;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class PetDTOMapperTest
    {
        [Test]
        public void TestToDTOPet()
        {
            var corePet = new CorePet();

            corePet.Id = 1;
            corePet.OwnerId = 1;
            corePet.BreedId = 1;
            corePet.PetTypeId = 1;
            corePet.Owner = new Client() { Id = 1, FirstName = "John", LastName = "Doe" };
            corePet.BreedInfo = new Breed() { Id = 1, BreedName = "Corgi" };
            corePet.PetType = new PetType() { Id = 1, PetTypeName = "Dog" };
            corePet.Dob = "1/1/2022";
            corePet.Weight = 30;
            corePet.Vaccines = new List<VaccineStatus>()
            {
                new VaccineStatus()
                {
                    Id = 1,
                    PetToVaccineId = 1,
                    VaxName = "Bordetella",
                    Inoculated = true
                }
            };

            var dtoPet = PetDTOMapper.ToDTOPet(corePet);

            Assert.IsNotNull(dtoPet);
            Assert.AreEqual(1, dtoPet.Id);
            Assert.AreEqual(1, dtoPet.OwnerId);
            Assert.AreEqual(1, dtoPet.BreedId);
            Assert.AreEqual(1, dtoPet.PetTypeId);
            Assert.AreEqual("John", dtoPet.OwnerFirstName);
            Assert.AreEqual("Doe", dtoPet.OwnerLastName);
            Assert.AreEqual("1/1/2022", dtoPet.Dob);
            Assert.AreEqual("Corgi", dtoPet.BreedName);
            Assert.AreEqual("Dog", dtoPet.PetTypeName);
            Assert.AreEqual(30, dtoPet.Weight);

            Assert.IsNotNull(dtoPet.Vaccines);
            Assert.AreEqual(1, dtoPet.Vaccines.Count);

            var vax = dtoPet.Vaccines[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetsVaccineId);
            Assert.AreEqual("Bordetella", vax.VaccineName);
            Assert.IsTrue(vax.Innoculated);
        }

        [Test]
        public void TestFromDTOPet()
        {
            var dtoPet = new DTOPet();

            dtoPet.Id = 1;
            dtoPet.OwnerId = 1;
            dtoPet.PetTypeId = 1;
            dtoPet.BreedId = 1;
            dtoPet.OwnerFirstName = "John";
            dtoPet.OwnerLastName = "Doe";
            dtoPet.BreedName = "Corgi";
            dtoPet.PetTypeName = "Dog";
            dtoPet.Dob = "1/1/2022";
            dtoPet.Weight = 30;
            dtoPet.Vaccines = new List<PetsVaccineDTO>()
            {
                new PetsVaccineDTO()
                {
                    Id = 1,
                    PetsVaccineId = 1,
                    VaccineName = "Bordetella",
                    Innoculated = true
                }
            };

            var corePet = PetDTOMapper.FromDTOPet(dtoPet);

            Assert.IsNotNull(corePet);
            Assert.AreEqual(1, corePet.Id);
            Assert.AreEqual(1, corePet.OwnerId);
            Assert.AreEqual(1, corePet.BreedId);
            Assert.AreEqual(1, corePet.PetTypeId);

            Assert.IsNotNull(corePet.Owner);
            Assert.AreEqual(1, corePet.Owner.Id);
            Assert.AreEqual("John", corePet.Owner.FirstName);
            Assert.AreEqual("Doe", corePet.Owner.LastName);
            Assert.AreEqual("1/1/2022", corePet.Dob);
            
            Assert.IsNotNull(corePet.BreedInfo);
            Assert.AreEqual(1, corePet.BreedInfo.Id);
            Assert.AreEqual("Corgi", corePet.BreedInfo.BreedName);

            Assert.IsNotNull(corePet.PetType);
            Assert.AreEqual(1, corePet.PetType.Id);
            Assert.AreEqual("Dog", corePet.PetType.PetTypeName);

            Assert.AreEqual(30, corePet.Weight);

            Assert.IsNotNull(corePet.Vaccines);
            Assert.AreEqual(1, corePet.Vaccines.Count);
            var vax = corePet.Vaccines[0];

            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetToVaccineId);
            Assert.AreEqual("Bordetella", vax.VaxName);
            Assert.IsTrue(vax.Inoculated);
        }
    }
}
