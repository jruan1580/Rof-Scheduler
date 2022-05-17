using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IClientService
    {
        Task ClientLogin(string email, string password);
        Task ClientLogout(long id);
        Task CreateClient(Client newClient, string password);
        Task DeleteClientById(long id);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);
        Task IncrementClientFailedLoginAttempts(long id);
        Task ResetClientFailedLoginAttempts(long id);
        Task UpdateClientInfo(Client client);
        Task UpdatePassword(long id, string newPassword);
    }

    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IPasswordService _passwordService;

        public ClientService(IClientRepository clientRepository, IPasswordService passwordService)
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

            var clientCheck = await _clientRepository.GetClientByEmail(newClient.EmailAddress);

            if (clientCheck != null && clientCheck.FirstName == newClient.FirstName && clientCheck.LastName == newClient.LastName)
            {
                throw new ArgumentException("Client with this name and email address already exists.");
            }

            if (!_passwordService.VerifyPasswordRequirements(password))
            {
                throw new ArgumentException("Password does not meet requirements");
            }

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

            var clientCheck = await GetClientByEmail(client.EmailAddress);
            if (clientCheck != null && clientCheck.Id != client.Id && clientCheck.FirstName == client.FirstName && clientCheck.LastName == client.LastName)
            {
                throw new ArgumentException("Email address and name already in use.");
            }

            var origClient = await _clientRepository.GetClientById(client.Id);
            if (origClient == null)
            {
                throw new ArgumentException($"Client with id: {client.Id} does not exist.");
            }

            origClient.FirstName = client.FirstName;
            origClient.LastName = client.LastName;
            origClient.EmailAddress = client.EmailAddress;
            origClient.PrimaryPhoneNum = client.PrimaryPhoneNum;
            origClient.AddressLine1 = client.Address?.AddressLine1;
            origClient.AddressLine2 = client.Address?.AddressLine2;
            origClient.City = client.Address?.City;
            origClient.State = client.Address?.State;
            origClient.ZipCode = client.Address?.ZipCode;

            await _clientRepository.UpdateClientInfo(origClient);
        }

        public async Task<Client> GetClientById(long id)
        {
            var client = await _clientRepository.GetClientById(id);

            if (client == null)
            {
                throw new ArgumentException("Client does not exist.");
            }

            return ClientMapper.ToCoreClient(client);
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            var client = await _clientRepository.GetClientByEmail(email);

            if (client == null)
            {
                return null;
            }

            return ClientMapper.ToCoreClient(client);
        }

        public async Task ClientLogin(string email, string password)
        {
            var client = await GetClientByEmail(email);

            if (client == null)
            {
                throw new ArgumentException($"Client with email: {email} not found.");
            }

            if (!_passwordService.VerifyPasswordHash(password, client.Password))
            {
                await IncrementClientFailedLoginAttempts(client.Id);

                throw new ArgumentException("Incorrect password.");
            }

            if (client.IsLoggedIn)
            {
                throw new ArgumentException("Client is already logged in.");
            }

            if (client.IsLocked)
            {
                throw new ArgumentException("Client account is locked. Unable to log in.");
            }

            await _clientRepository.UpdateClientLoginStatus(client.Id, true);

            client.IsLoggedIn = true;
        }

        public async Task ClientLogout(long id)
        {
            var client = await GetClientById(id);

            if (!client.IsLoggedIn)
            {
                throw new ArgumentException("Client already logged out.");
            }

            await _clientRepository.UpdateClientLoginStatus(client.Id, false);

            client.IsLoggedIn = false;
        }

        public async Task IncrementClientFailedLoginAttempts(long id)
        {
            var client = await GetClientById(id);

            if (client.IsLocked)
            {
                return;
            }

            var attempts = await _clientRepository.IncrementClientFailedLoginAttempts(client.Id);

            if (attempts != 3)
            {
                return;
            }

            await _clientRepository.UpdateClientIsLocked(client.Id, true);
        }

        public async Task ResetClientFailedLoginAttempts(long id)
        {
            await _clientRepository.ResetClientFailedLoginAttempts(id);

            await _clientRepository.UpdateClientIsLocked(id, false);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var client = await GetClientById(id);

            if (!_passwordService.VerifyPasswordRequirements(newPassword))
            {
                throw new ArgumentException("New password does not meet all requirements.");
            }

            if (_passwordService.VerifyPasswordHash(newPassword, client.Password))
            {
                throw new ArgumentException("New password cannot be the same as current password.");
            }

            var newEncryptedPass = _passwordService.EncryptPassword(newPassword);

            await _clientRepository.UpdatePassword(client.Id, newEncryptedPass);
        }

        public async Task DeleteClientById(long id)
        {
            await _clientRepository.DeleteClientById(id);
        }
    }
}
