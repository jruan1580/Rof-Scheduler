using ClientManagementService.API.DTOMapper;
using NUnit.Framework;
using CoreClient = ClientManagementService.Domain.Models.Client;
using DTOClient = ClientManagementService.API.DTO.ClientDTO;

namespace ClientManagementService.Test.Mapper
{
    [TestFixture]
    public class ClientDTOMapperTest
    {
        [Test]
        public void TestToDTOClient()
        {
            var coreClient = new CoreClient();

            coreClient.Id = 1;
            coreClient.CountryId = 1;
            coreClient.FirstName = "John";
            coreClient.LastName = "Doe";
            coreClient.EmailAddress = "jdoe@gmail.com";
            coreClient.PrimaryPhoneNum = "123-456-7890";
            coreClient.IsLocked = false;
            coreClient.FailedLoginAttempts = 0;
            coreClient.TempPasswordChanged = true;
            coreClient.IsLoggedIn = false;
            coreClient.SetFullName();

            coreClient.Address = new Domain.Models.Address();
            coreClient.Address.AddressLine1 = "123 Test St";
            coreClient.Address.AddressLine2 = null;
            coreClient.Address.City = "San Diego";
            coreClient.Address.State = "CA";
            coreClient.Address.ZipCode = "12345";


            var dtoClient = ClientDTOMapper.ToDTOClient(coreClient);

            Assert.IsNotNull(dtoClient);
            Assert.AreEqual(1, dtoClient.Id);
            Assert.AreEqual("John", dtoClient.FirstName);
            Assert.AreEqual("Doe", dtoClient.LastName);
            Assert.AreEqual("123-456-7890", dtoClient.PrimaryPhoneNum);
            Assert.AreEqual("jdoe@gmail.com", dtoClient.EmailAddress);
            Assert.IsFalse(dtoClient.IsLocked);
            Assert.AreEqual(0, dtoClient.FailedLoginAttempts);
            Assert.IsTrue(dtoClient.TempPasswordChanged);
            Assert.IsFalse(dtoClient.IsLoggedIn);
            Assert.AreEqual("John Doe", dtoClient.FullName);
            Assert.AreEqual("123 Test St", dtoClient.Address?.AddressLine1);
            Assert.IsNull(dtoClient.Address?.AddressLine2);
            Assert.AreEqual("CA", dtoClient.Address?.State);
            Assert.AreEqual("San Diego", dtoClient.Address?.City);
            Assert.AreEqual("12345", dtoClient.Address?.ZipCode);
        }

        [Test]
        public void TestFromDTOClient()
        {
            var dtoClient = new DTOClient();

            dtoClient.Id = 1;
            dtoClient.FirstName = "John";
            dtoClient.LastName = "Doe";
            dtoClient.PrimaryPhoneNum = "123-456-7890";
            dtoClient.EmailAddress = "jdoe@gmail.com";
            dtoClient.IsLocked = false;
            dtoClient.FailedLoginAttempts = 0;
            dtoClient.TempPasswordChanged = true;
            dtoClient.IsLoggedIn = false;

            dtoClient.Address = new API.DTO.AddressDTO();
            dtoClient.Address.AddressLine1 = "123 Test St";
            dtoClient.Address.AddressLine2 = null;
            dtoClient.Address.City = "San Diego";
            dtoClient.Address.State = "CA";
            dtoClient.Address.ZipCode = "12345";


            var coreClient = ClientDTOMapper.FromDTOClient(dtoClient);

            Assert.IsNotNull(coreClient);
            Assert.AreEqual(1, coreClient.Id);
            Assert.AreEqual("John", coreClient.FirstName);
            Assert.AreEqual("Doe", coreClient.LastName);
            Assert.AreEqual("123-456-7890", coreClient.PrimaryPhoneNum);
            Assert.AreEqual("jdoe@gmail.com", coreClient.EmailAddress);
            Assert.IsFalse(coreClient.IsLocked);
            Assert.AreEqual(0, coreClient.FailedLoginAttempts);
            Assert.IsTrue(coreClient.TempPasswordChanged);
            Assert.IsFalse(coreClient.IsLoggedIn);
            Assert.AreEqual("John Doe", coreClient.FullName);
            Assert.AreEqual("123 Test St", coreClient.Address?.AddressLine1);
            Assert.IsNull(coreClient.Address?.AddressLine2);
            Assert.AreEqual("CA", coreClient.Address?.State);
            Assert.AreEqual("San Diego", coreClient.Address?.City);
            Assert.AreEqual("12345", coreClient.Address?.ZipCode);
        }
    }
}
