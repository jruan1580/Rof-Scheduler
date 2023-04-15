using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using System;
using System.Threading.Tasks;
using ClientDB = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using RofShared.Exceptions;
using RofShared.Services;
using System.Net;

namespace ClientManagementService.Domain.Services
{
    public interface IClientService
    {
        Task<Client> ClientLogin(string email, string password);
        Task ClientLogout(long id);
        Task CreateClient(Client newClient, string password);
        Task DeleteClientById(long id);      
        Task ResetClientFailedLoginAttempts(long id);
        Task UpdateClientInfo(Client client);
        Task UpdatePassword(long id, string newPassword);
    }

    public class ClientService : ClientBaseService, IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IPasswordService _passwordService;

        public ClientService(IClientRepository clientRepository, IPasswordService passwordService, IClientRetrievalRepository clientRetrievalRepository)
            : base(clientRetrievalRepository)
        {
            _clientRepository = clientRepository;
            _passwordService = passwordService;
        }

        public async Task CreateClient(Client newClient, string password)
        {
            var invalidErrs = newClient.IsValidClientToCreate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var clientExists = await _clientRetrievalRepository.DoesClientExistByEmailOrUsername(newClient.Id, newClient.EmailAddress, newClient.Username);
            if (clientExists)
            {
                throw new ArgumentException("An account with either username or email already exists.");
            }

            _passwordService.ValidatePasswordForCreate(password);

            var encryptedPass = _passwordService.EncryptPassword(password);
            newClient.Password = encryptedPass;

            var newClientEntity = ClientMapper.FromCoreClient(newClient);

            await _clientRepository.CreateClient(newClientEntity);
        }

        public async Task UpdateClientInfo(Client client)
        {
            var invalidErrs = client.IsValidClientToUpdate().ToArray();

            if (invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            var clientExists = await _clientRetrievalRepository.DoesClientExistByEmailOrUsername(client.Id, client.EmailAddress, client.Username);
            if (clientExists)
            {
                throw new ArgumentException("An account with either username or email already exists.");
            }

            var origClient = await GetDbClientById(client.Id);

            origClient.FirstName = client.FirstName;
            origClient.LastName = client.LastName;
            origClient.EmailAddress = client.EmailAddress;
            origClient.PrimaryPhoneNum = client.PrimaryPhoneNum;
            origClient.AddressLine1 = client.Address?.AddressLine1;
            origClient.AddressLine2 = client.Address?.AddressLine2;
            origClient.City = client.Address?.City;
            origClient.State = client.Address?.State;
            origClient.ZipCode = client.Address?.ZipCode;

            await _clientRepository.UpdateClient(origClient);
        }

        public async Task<Client> ClientLogin(string username, string password)
        {
            var client = await GetDbClientByUsername(username);

            if (client.IsLocked)
            {
                throw new ArgumentException("Client account is locked. Contact admin to get unlocked.");
            }

            await VerifyLoginPasswordAndIncrementFailedLoginAttemptsIfFail(password, client);

            if (client.IsLoggedIn)
            {
                return ClientMapper.ToCoreClient(client);
            }

            client.IsLoggedIn = true;
            
            await _clientRepository.UpdateClient(client);

            return ClientMapper.ToCoreClient(client);
        }

        public async Task ClientLogout(long id)
        {
            var client = await GetDbClientById(id);

            if (!client.IsLoggedIn)
            {
                return;
            }

            client.IsLoggedIn = false;

            await _clientRepository.UpdateClient(client);
        }   

        public async Task ResetClientFailedLoginAttempts(long id)
        {
            var client = await GetDbClientById(id);

            client.FailedLoginAttempts = 0;
            client.IsLocked = false;

            await _clientRepository.UpdateClient(client);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var client = await GetDbClientById(id);

            _passwordService.ValidateNewPasswordForUpdate(newPassword, client.Password);

            var newEncryptedPass = _passwordService.EncryptPassword(newPassword);

            client.Password = newEncryptedPass;

            await _clientRepository.UpdateClient(client);
        }

        public async Task DeleteClientById(long id)
        {
            await _clientRepository.DeleteClientById(id);
        }

        private async Task IncrementClientFailedLoginAttempts(ClientDB client)
        {
            if (client.IsLocked)
            {
                return;
            }

            var attempts = await _clientRepository.IncrementClientFailedLoginAttempts(client.Id);

            if (attempts != 3)
            {
                return;
            }

            client.IsLocked = true;
            client.FailedLoginAttempts = attempts;

            await _clientRepository.UpdateClient(client);
        }

        private async Task VerifyLoginPasswordAndIncrementFailedLoginAttemptsIfFail(string password, ClientDB client)
        {
            try
            {
                _passwordService.ValidatePasswordForLogin(password, client.Password);
            }
            catch (ArgumentException)
            {
                //password was incorrect
                await IncrementClientFailedLoginAttempts(client);

                throw;
            }
        }
    }
}
