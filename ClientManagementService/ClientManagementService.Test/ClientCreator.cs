using ClientManagementService.API.DTO;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using DomainClient = ClientManagementService.Domain.Models.Client;

namespace ClientManagementService.Test
{
    public static class ClientCreator
    {
        public static DbClient GetDbClient(byte[] encryptedPassword)
        {
            return new DbClient()
            {
                Id = 1,
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "jdoe",
                EmailAddress = "test@email.com",
                Password = encryptedPassword,
                TempPasswordChanged = false,
                AddressLine1 = "123 Abc St",
                AddressLine2 = "",
                City = "Oakland",
                State = "CA",
                ZipCode = "12345"
            };
        }

        public static DomainClient GetDomainClient(byte[] encryptedPassword)
        {
            return new DomainClient()
            {
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = encryptedPassword,
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };
        }

        public static ClientDTO GetClientDTO(string role)
        {
            return new ClientDTO()
            {
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Password = "TestPassword123!",
                PrimaryPhoneNum = "123-456-7890",
                IsLoggedIn = false,
                IsLocked = false,
                FailedLoginAttempts = 0,
                TempPasswordChanged = false,
                Address = new AddressDTO()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };
        }
    }
}
