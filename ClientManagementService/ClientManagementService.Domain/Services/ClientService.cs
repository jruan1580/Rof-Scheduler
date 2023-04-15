using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using ClientDB = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using RofShared.Services;

namespace ClientManagementService.Domain.Services
{
    public interface IClientService
    {
        Task<Client> ClientLogin(string email, string password);
        Task ClientLogout(long id);
    }

    public class ClientService : ClientBaseService, IClientService
    {
        private readonly IClientUpsertRepository _clientRepository;
        private readonly IPasswordService _passwordService;

        public ClientService(IClientUpsertRepository clientRepository, IPasswordService passwordService, IClientRetrievalRepository clientRetrievalRepository)
            : base(clientRetrievalRepository)
        {
            _clientRepository = clientRepository;
            _passwordService = passwordService;
        }

        public async Task<Client> ClientLogin(string username, string password)
        {
            var client = await GetDbClientByUsername(username);

            if (client.IsLocked)
            {
                throw new ArgumentException("Client account is locked. Contact admin to get unlocked.");
            }

            _passwordService.ValidatePasswordForLogin(password, client.Password);

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
