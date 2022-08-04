using NUnit.Framework;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using CorePet = ClientManagementService.Domain.Models.Pet;
using ClientManagementService.Domain.Mappers.Database;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using DbBreed = ClientManagementService.Infrastructure.Persistence.Entities.Breed;
using CoreClient = ClientManagementService.Domain.Models.Client;
using CoreBreed = ClientManagementService.Domain.Models.Breed;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class PetMapperTest
    {
        [Test]
        public void ToCorePetTest()
        {
            var entity = new DbPet();

            entity.Id = 1;
            entity.OwnerId = 1;
            entity.BreedId = 1;
            entity.Owner = new DbClient()
            {
                FirstName = "John",
                LastName = "Doe"
            };
            entity.Dob = "1/1/2022";
            entity.Weight = 30;
            entity.Breed = new DbBreed() { BreedName = "Corgi" };

            var core = PetMapper.ToCorePet(entity);

            Assert.IsNotNull(core);
            Assert.AreEqual(1, core.Id);
            Assert.AreEqual(1, core.OwnerId);
            Assert.AreEqual(1, core.BreedId);
            Assert.AreEqual("John", core.Owner.FirstName);
            Assert.AreEqual("Doe", core.Owner.LastName);
            Assert.AreEqual("1/1/2022", core.Dob);
            Assert.AreEqual("Corgi", core.BreedInfo.BreedName);
            Assert.AreEqual(30, core.Weight);
        }

        [Test]
        public void FromCorePetTest()
        {
            var core = new CorePet();

            core.Id = 1;
            core.OwnerId = 1;
            core.BreedId = 1;
            core.Dob = "1/1/2022";
            core.Weight = 30;

            var entity = PetMapper.FromCorePet(core);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual(1, entity.OwnerId);
            Assert.AreEqual(1, entity.BreedId);
            Assert.AreEqual("1/1/2022", entity.Dob);
            Assert.AreEqual(30, entity.Weight);
        }
    }
}
