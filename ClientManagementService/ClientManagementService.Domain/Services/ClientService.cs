using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientDB = ClientManagementService.Infrastructure.Persistence.Entities.Client;

namespace ClientManagementService.Domain.Services
{
    public interface IClientService
    {
        Task<Client> ClientLogin(string email, string password);
        Task ClientLogout(long id);
        Task CreateClient(Client newClient, string password);
        Task DeleteClientById(long id);
        Task<ClientsWithTotalPage> GetAllClientsByKeyword(int page, int offset, string keyword);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);        
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

            var clientExists = await _clientRepository.ClientAlreadyExists(newClient.Id, newClient.EmailAddress, newClient.FirstName, newClient.LastName, newClient.Username);
            if (clientExists)
            {
                throw new ArgumentException("Either username already exists or email address and name combination already exists");
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

            var clientExists = await _clientRepository.ClientAlreadyExists(client.Id, client.EmailAddress, client.FirstName, client.LastName, client.Username);
            if (clientExists)
            {
                throw new ArgumentException("Either username already exists or email address and name combination already exists");
            }

            var origClient = await _clientRepository.GetClientByFilter(new GetClientFilterModel<long>(GetClientFilterEnum.Id, client.Id));
            if (origClient == null)
            {
                throw new ClientNotFoundException();
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

            await _clientRepository.UpdateClient(origClient);
        }

        public async Task<ClientsWithTotalPage> GetAllClientsByKeyword(int page, int offset, string keyword)
        {
            var result = await _clientRepository.GetAllClientsByKeyword(page, offset, keyword);
            var clients = result.Item1;
            var totalPages = result.Item2;

            if (clients == null || clients.Count == 0)
            {
                return new ClientsWithTotalPage(new List<Client>(), 0);
            }

            return new ClientsWithTotalPage(clients.Select(c => ClientMapper.ToCoreClient(c)).ToList(), totalPages);
        }

        public async Task<Client> GetClientById(long id)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<long>(GetClientFilterEnum.Id, id));

            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            return ClientMapper.ToCoreClient(client);
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<string>(GetClientFilterEnum.Email, email));

            if (client == null)
            {
                return null;
            }

            return ClientMapper.ToCoreClient(client);
        }       

        public async Task<Client> ClientLogin(string username, string password)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<string>(GetClientFilterEnum.Username, username));

            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            if (client.IsLocked)
            {
                throw new ArgumentException("Client account is locked. Contact admin to get unlocked.");
            }

            if (client.IsLoggedIn)
            {
                return ClientMapper.ToCoreClient(client);
            }

            if (!_passwordService.VerifyPasswordHash(password, client.Password))
            {
                await IncrementClientFailedLoginAttempts(client);

                throw new ArgumentException("Incorrect password.");
            }

            client.IsLoggedIn = true;
            
            await _clientRepository.UpdateClient(client);

            return ClientMapper.ToCoreClient(client);
        }

        public async Task ClientLogout(long id)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<long>(GetClientFilterEnum.Id, id));

            if (!client.IsLoggedIn)
            {
                return;
            }

            client.IsLoggedIn = false;

            await _clientRepository.UpdateClient(client);
        }   

        public async Task ResetClientFailedLoginAttempts(long id)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<long>(GetClientFilterEnum.Id, id));

            if(client == null)
            {
                throw new ClientNotFoundException();
            }

            client.FailedLoginAttempts = 0;
            client.IsLocked = false;

            await _clientRepository.UpdateClient(client);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var client = await _clientRepository.GetClientByFilter(new GetClientFilterModel<long>(GetClientFilterEnum.Id, id));

            if (!_passwordService.VerifyPasswordRequirements(newPassword))
            {
                throw new ArgumentException("New password does not meet all requirements.");
            }

            if (_passwordService.VerifyPasswordHash(newPassword, client.Password))
            {
                throw new ArgumentException("New password cannot be the same as current password.");
            }

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
    }
}
