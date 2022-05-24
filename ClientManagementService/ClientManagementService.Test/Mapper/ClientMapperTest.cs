using ClientManagementService.Domain.Mappers.Database;
using NUnit.Framework;
using CoreClient = ClientManagementService.Domain.Models.Client;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class ClientMapperTest
    {
        [Test]
        public void ToCoreClientTest()
        {
            var entity = new DbClient();

            entity.Id = 1;
            entity.CountryId = 1;
            entity.FirstName = "John";
            entity.LastName = "Doe";
            entity.EmailAddress = "jdoe@gmail.com";
            entity.Username = "jdoe";
            entity.PrimaryPhoneNum = "123-456-7890";
            entity.Password = new byte[32];
            entity.IsLocked = false;
            entity.FailedLoginAttempts = 0;
            entity.TempPasswordChanged = true;
            entity.IsLoggedIn = false;
            entity.AddressLine1 = "123 Test St.";
            entity.City = "San Diego";
            entity.State = "CA";
            entity.ZipCode = "12345";

            var core = ClientMapper.ToCoreClient(entity);

            Assert.IsNotNull(core);
            Assert.AreEqual(1, core.Id);
            Assert.AreEqual(1, core.CountryId);
            Assert.AreEqual("John", core.FirstName);
            Assert.AreEqual("Doe", core.LastName);
            Assert.AreEqual("jdoe@gmail.com", core.EmailAddress);
            Assert.AreEqual("jdoe", core.Username);
            Assert.AreEqual("123-456-7890", core.PrimaryPhoneNum);
            Assert.AreEqual(new byte[32], core.Password);
            Assert.IsFalse(core.IsLocked);
            Assert.AreEqual(0, core.FailedLoginAttempts);
            Assert.IsTrue(core.TempPasswordChanged);
            Assert.IsFalse(core.IsLoggedIn);
            Assert.AreEqual("John Doe", core.FullName);
            Assert.AreEqual("123 Test St.", core.Address.AddressLine1);
            Assert.IsNull(core.Address.AddressLine2);
            Assert.AreEqual("San Diego", core.Address.City);
            Assert.AreEqual("CA", core.Address.State);
            Assert.AreEqual("12345", core.Address.ZipCode);
        }

        [Test]
        public void FromCoreClientTest()
        {
            var core = new CoreClient();

            core.Id = 1;
            core.CountryId = 1;
            core.FirstName = "John";
            core.LastName = "Doe";
            core.EmailAddress = "jdoe@gmail.com";
            core.Username = "jdoe";
            core.PrimaryPhoneNum = "123-456-7890";
            core.Password = new byte[32];
            core.IsLocked = false;
            core.FailedLoginAttempts = 0;
            core.TempPasswordChanged = true;
            core.IsLoggedIn = false;

            core.SetAddress("123 Test St.", null, "San Diego", "CA", "12345");

            var entity = ClientMapper.FromCoreClient(core);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual(1, entity.CountryId);
            Assert.AreEqual("John", entity.FirstName);
            Assert.AreEqual("Doe", entity.LastName);
            Assert.AreEqual("jdoe@gmail.com", entity.EmailAddress);
            Assert.AreEqual("jdoe", entity.Username);
            Assert.AreEqual("123-456-7890", entity.PrimaryPhoneNum);
            Assert.AreEqual(new byte[32], entity.Password);
            Assert.IsFalse(entity.IsLocked);
            Assert.AreEqual(0, entity.FailedLoginAttempts);
            Assert.True(entity.TempPasswordChanged);
            Assert.IsFalse(entity.IsLoggedIn);
            Assert.AreEqual("123 Test St.", entity.AddressLine1);
            Assert.IsNull(entity.AddressLine2);
            Assert.AreEqual("San Diego", entity.City);
            Assert.AreEqual("CA", entity.State);
            Assert.AreEqual("12345", entity.ZipCode);
        }
    }
}
