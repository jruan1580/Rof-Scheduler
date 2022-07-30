using CoreClient = ClientManagementService.Domain.Models.Client;
using ClientManagementService.API.DTO;

namespace ClientManagementService.API.DTOMapper
{
    public static class ClientDTOMapper
    {
        public static ClientDTO ToDTOClient(CoreClient coreClient)
        {
            var dtoClient = new ClientDTO();

            dtoClient.Id = coreClient.Id;
            dtoClient.CountryId = coreClient.CountryId;
            dtoClient.FirstName = coreClient.FirstName;
            dtoClient.LastName = coreClient.LastName;
            dtoClient.EmailAddress = coreClient.EmailAddress;
            dtoClient.Username = coreClient.Username;
            dtoClient.PrimaryPhoneNum = coreClient.PrimaryPhoneNum;
            dtoClient.SecondaryPhoneNum = coreClient.SecondaryPhoneNum;
            dtoClient.IsLocked = coreClient.IsLocked;
            dtoClient.FailedLoginAttempts = coreClient.FailedLoginAttempts;
            dtoClient.TempPasswordChanged = coreClient.TempPasswordChanged;
            dtoClient.IsLoggedIn = coreClient.IsLoggedIn;
            dtoClient.FullName = coreClient.FullName;

            dtoClient.Address = new AddressDTO();
            dtoClient.Address.AddressLine1 = coreClient.Address?.AddressLine1;
            dtoClient.Address.AddressLine2 = coreClient.Address?.AddressLine2;
            dtoClient.Address.City = coreClient.Address?.City;
            dtoClient.Address.State = coreClient.Address?.State;
            dtoClient.Address.ZipCode = coreClient.Address?.ZipCode;


            return dtoClient;
        }

        public static CoreClient FromDTOClient(ClientDTO dtoClient)
        {
            var coreClient = new CoreClient();

            coreClient.Id = dtoClient.Id;
            coreClient.CountryId = dtoClient.CountryId;
            coreClient.FirstName = dtoClient.FirstName;
            coreClient.LastName = dtoClient.LastName;

            coreClient.SetFullName();

            coreClient.EmailAddress = dtoClient.EmailAddress;
            coreClient.Username = dtoClient.Username;
            coreClient.PrimaryPhoneNum = dtoClient.PrimaryPhoneNum;
            coreClient.SecondaryPhoneNum = dtoClient.SecondaryPhoneNum;
            coreClient.IsLocked = dtoClient.IsLocked;
            coreClient.FailedLoginAttempts = dtoClient.FailedLoginAttempts;
            coreClient.TempPasswordChanged = dtoClient.TempPasswordChanged;
            coreClient.IsLoggedIn = dtoClient.IsLoggedIn;

            coreClient.SetAddress(dtoClient.Address?.AddressLine1, dtoClient.Address?.AddressLine2, dtoClient.Address?.City, dtoClient.Address?.State, dtoClient.Address?.ZipCode);

            return coreClient;
        }
    }
}
