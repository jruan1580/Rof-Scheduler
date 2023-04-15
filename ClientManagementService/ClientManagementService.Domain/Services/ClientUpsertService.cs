using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using RofShared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DBClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;

namespace ClientManagementService.Domain.Services
{
    public class ClientUpsertService : ClientBaseService
    {
        private readonly IClientUpsertRepository _clientUpsertRepository;

        private readonly IPasswordService _passwordService;

        public ClientUpsertService(IClientRetrievalRepository clientRetrievalRepository,
            IClientUpsertRepository clientUpsertRepository, 
            IPasswordService passwordService) : base(clientRetrievalRepository)
        {
            _clientUpsertRepository = clientUpsertRepository;

            _passwordService = passwordService;
        }

        public async Task CreateClient(Client newClient, string password)
        {
            await ValidateClient(newClient, false);

            _passwordService.ValidatePasswordForCreate(password);

            newClient.Password = _passwordService.EncryptPassword(password);

            var newClientDB = ClientMapper.FromCoreClient(newClient);

            await _clientUpsertRepository.CreateClient(newClientDB);
        }

        public async Task UpdateClientInformation(Client client)
        {
            await ValidateClient(client, true);

            var originalClient = await GetDbClientById(client.Id);

            MergeClientPropertiesForUpdate(originalClient, client);

            await _clientUpsertRepository.UpdateClient(originalClient);
        }

        public async Task ResetClientFailedLoginAttempts(long id)
        {
            var client = await GetDbClientById(id);

            client.FailedLoginAttempts = 0;
            client.IsLocked = false;

            await _clientUpsertRepository.UpdateClient(client);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var client = await GetDbClientById(id);

            _passwordService.ValidateNewPasswordForUpdate(newPassword, client.Password);

            client.Password = _passwordService.EncryptPassword(newPassword);

            await _clientUpsertRepository.UpdateClient(client);
        }

        public async Task DeleteClientById(long id)
        {
            await _clientUpsertRepository.DeleteClientById(id);
        }

        private async Task ValidateClient(Client client, bool isUpdate)
        {
            ValidateClientProperties(client, isUpdate);

            await ValidateIfClientIsDuplicate(client.Id, client.EmailAddress, client.Username);
        }

        private void ValidateClientProperties(Client client, bool isUpdate)
        {
            var validationErrors = (isUpdate) ? client.GetValidationErrorsForUpdate() : client.GetValidationErrorsForBothCreateOrUpdate();

            if (validationErrors.Count > 0)
            {
                var errorMessage = string.Join("\n", validationErrors);

                throw new ArgumentException(errorMessage);
            }
        }

        private async Task ValidateIfClientIsDuplicate(long id, string email, string username)
        {
            var isDuplicate = await _clientRetrievalRepository.DoesClientExistByEmailOrUsername(id, email, username);

            if (isDuplicate)
            {
                throw new ArgumentException("Client with email or username exists");
            }
        }

        private void MergeClientPropertiesForUpdate(DBClient originalClient, Client updatedClient)
        {
            originalClient.FirstName = updatedClient.FirstName;
            originalClient.LastName = updatedClient.LastName;
            originalClient.EmailAddress = updatedClient.EmailAddress;
            originalClient.Username = updatedClient.Username;
            originalClient.PrimaryPhoneNum = updatedClient.PrimaryPhoneNum;
            originalClient.SecondaryPhoneNum = updatedClient.SecondaryPhoneNum;
            originalClient.AddressLine1 = updatedClient.Address?.AddressLine1;
            originalClient.AddressLine2 = updatedClient.Address?.AddressLine2;
            originalClient.State = updatedClient.Address?.State;
            originalClient.City = updatedClient.Address?.City;
            originalClient.ZipCode = updatedClient.Address?.ZipCode;
        }
    }
}
