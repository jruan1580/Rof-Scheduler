using CoreClient = ClientManagementService.Domain.Models.Client;
using DbClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;

namespace ClientManagementService.Domain.Mappers.Database
{
    public class ClientMapper
    {
        public static CoreClient ToCoreClient(DbClient dbClient)
        {
            var coreClient = new CoreClient();

            coreClient.Id = dbClient.Id;
            coreClient.CountryId = dbClient.CountryId;
            coreClient.FirstName = dbClient.FirstName;
            coreClient.LastName = dbClient.LastName;
            coreClient.EmailAddress = dbClient.EmailAddress;
            coreClient.Username = dbClient.Username;
            coreClient.Password = dbClient.Password;
            coreClient.PrimaryPhoneNum = dbClient.PrimaryPhoneNum;
            coreClient.SecondaryPhoneNum = dbClient.SecondaryPhoneNum;
            coreClient.IsLocked = dbClient.IsLocked;
            coreClient.TempPasswordChanged = dbClient.TempPasswordChanged;
            coreClient.FailedLoginAttempts = dbClient.FailedLoginAttempts;
            coreClient.IsLoggedIn = dbClient.IsLoggedIn;

            coreClient.SetAddress(dbClient.AddressLine1, dbClient.AddressLine2, dbClient.City, dbClient.State, dbClient.ZipCode);

            coreClient.SetFullName();

            return coreClient;
        }

        public static DbClient FromCoreClient(CoreClient coreClient)
        {
            var entity = new DbClient();

            entity.Id = coreClient.Id;
            entity.CountryId = coreClient.CountryId;
            entity.FirstName = coreClient.FirstName;
            entity.LastName = coreClient.LastName;
            entity.EmailAddress = coreClient.EmailAddress;
            entity.Username = coreClient.Username;
            entity.Password = coreClient.Password;
            entity.PrimaryPhoneNum = coreClient.PrimaryPhoneNum;
            entity.SecondaryPhoneNum = coreClient.SecondaryPhoneNum;
            entity.IsLocked = coreClient.IsLocked;
            entity.TempPasswordChanged = coreClient.TempPasswordChanged;
            entity.FailedLoginAttempts = coreClient.FailedLoginAttempts;
            entity.IsLoggedIn = coreClient.IsLoggedIn;

            entity.AddressLine1 = coreClient.Address?.AddressLine1;
            entity.AddressLine2 = coreClient.Address?.AddressLine2;
            entity.City = coreClient.Address?.City;
            entity.State = coreClient.Address?.State;
            entity.ZipCode = coreClient.Address?.ZipCode;

            return entity;
        }
    }
}
