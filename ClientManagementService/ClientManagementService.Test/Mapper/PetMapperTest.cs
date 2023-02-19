using NUnit.Framework;
using DbPet = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using CorePet = ClientManagementService.Domain.Models.Pet;
using ClientManagementService.Domain.Mappers.Database;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using DbBreed = ClientManagementService.Infrastructure.Persistence.Entities.Breed;
using ClientManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;

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
            entity.PetTypeId = 1;
            entity.BreedId = 1;
            entity.Owner = new DbClient()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            entity.Dob = "1/1/2022";
            entity.Weight = 30;
            entity.Breed = new DbBreed() { Id = 1, BreedName = "Corgi" };
            entity.PetType = new PetType() { Id = 1, PetTypeName = "Dog" };

            var vaccines = new List<PetToVaccine>()
            {
                new PetToVaccine()
                {
                    Id = 1,
                    VaxId = 1,
                    Vax = new Vaccine()
                    {
                        Id = 1,
                        VaxName = "Bordetella"
                    },
                    Inoculated = true
                }
            };

            var core = PetMapper.ToCorePet(entity, vaccines);

            Assert.IsNotNull(core);
            Assert.AreEqual(1, core.Id);
            Assert.AreEqual(1, core.OwnerId);
            Assert.AreEqual(1, core.BreedId);
            Assert.AreEqual(1, core.PetTypeId);

            Assert.IsNotNull(core.Owner);
            Assert.AreEqual(1, core.Owner.Id);
            Assert.AreEqual("John", core.Owner.FirstName);
            Assert.AreEqual("Doe", core.Owner.LastName);

            Assert.IsNotNull(core.BreedInfo);
            Assert.AreEqual(1, core.BreedInfo.Id);
            Assert.AreEqual("Corgi", core.BreedInfo.BreedName);

            Assert.IsNotNull(core.PetType);
            Assert.AreEqual(1, core.PetType.Id);
            Assert.AreEqual("Dog", core.PetType.PetTypeName);

            Assert.AreEqual("1/1/2022", core.Dob);            
            Assert.AreEqual(30, core.Weight);

            Assert.IsNotNull(core.Vaccines);
            Assert.AreEqual(1, core.Vaccines.Count);

            var vax = core.Vaccines[0];
            Assert.AreEqual(1, vax.Id);
            Assert.AreEqual(1, vax.PetToVaccineId);
            Assert.AreEqual("Bordetella", vax.VaxName);
            Assert.IsTrue(vax.Inoculated);
        }

        [Test]
        public void FromCorePetTest()
        {
            var core = new CorePet();

            core.Id = 1;
            core.OwnerId = 1;
            core.PetTypeId = 1;
            core.BreedId = 1;
            core.Dob = "1/1/2022";
            core.Weight = 30;

            var entity = PetMapper.FromCorePet(core);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual(1, entity.OwnerId);
            Assert.AreEqual(1, entity.PetTypeId);
            Assert.AreEqual(1, entity.BreedId);
            Assert.AreEqual("1/1/2022", entity.Dob);
            Assert.AreEqual(30, entity.Weight);
        }

        [Test]
        public void ToCorePetTypeTest()
        {
            var entity = new PetType();

            entity.Id = 1;
            entity.PetTypeName = "Dog";

            var corePetType = PetMapper.ToCorePetType(entity);

            Assert.AreEqual(entity.Id, corePetType.Id);
            Assert.AreEqual(entity.PetTypeName, corePetType.PetTypeName);
        }

        [Test]
        public void ToCoreBreedTest()
        {
            var entity = new DbBreed();

            entity.Id = 1;
            entity.BreedName = "Golden Retriever";

            var core = PetMapper.ToCoreBreed(entity);

            Assert.IsNotNull(core);

            Assert.AreEqual(1, core.Id);
            Assert.AreEqual("Golden Retriever", core.BreedName);
        }

        [Test]
        public void ToCorePetDropdownTest()
        {
            var entity = new DbPet();

            entity.Id = 1;
            entity.Name = "TestPet";

            var core = PetMapper.ToCorePetDropDown(entity);

            Assert.IsNotNull(core);

            Assert.AreEqual(1, core.Id);
            Assert.AreEqual("TestPet", core.Name);
        }
    }
}
