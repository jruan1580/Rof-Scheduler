using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Models;
using NUnit.Framework;
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
            corePet.Owner = new Client() { FirstName = "John", LastName = "Doe" };
            corePet.BreedInfo = new Breed() { BreedName = "Corgi" };
            corePet.Dob = "1/1/2022";
            corePet.Weight = 30;


            var dtoPet = PetDTOMapper.ToDTOPet(corePet);

            Assert.IsNotNull(dtoPet);
            Assert.AreEqual(1, dtoPet.Id);
            Assert.AreEqual(1, dtoPet.OwnerId);
            Assert.AreEqual(1, dtoPet.BreedId);
            Assert.AreEqual("John", dtoPet.OwnerFirstName);
            Assert.AreEqual("Doe", dtoPet.OwnerLastName);
            Assert.AreEqual("1/1/2022", dtoPet.Dob);
            Assert.AreEqual("Corgi", dtoPet.BreedName);
            Assert.AreEqual(30, dtoPet.Weight);
        }

        [Test]
        public void TestFromDTOPet()
        {
            var dtoPet = new DTOPet();

            dtoPet.Id = 1;
            dtoPet.OwnerId = 1;
            dtoPet.BreedId = 1;
            dtoPet.OwnerFirstName = "John";
            dtoPet.OwnerLastName = "Doe";
            dtoPet.BreedName = "Corgi";
            dtoPet.Dob = "1/1/2022";
            dtoPet.Weight = 30;

            var corePet = PetDTOMapper.FromDTOPet(dtoPet);

            Assert.IsNotNull(corePet);
            Assert.AreEqual(1, corePet.Id);
            Assert.AreEqual(1, corePet.OwnerId);
            Assert.AreEqual(1, corePet.BreedId);
            Assert.AreEqual("John", corePet.Owner.FirstName);
            Assert.AreEqual("Doe", corePet.Owner.LastName);
            Assert.AreEqual("1/1/2022", corePet.Dob);
            Assert.AreEqual("Corgi", corePet.BreedInfo.BreedName);
            Assert.AreEqual(30, corePet.Weight);
        }
    }
}
