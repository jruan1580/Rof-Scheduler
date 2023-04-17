using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using ClientDB = ClientManagementService.Infrastructure.Persistence.Entities.Client;
using RofShared.Services;
using ClientManagementService.Domain.Exceptions;

namespace ClientManagementService.Domain.Services
{
    public class ClientAuthService : ClientService, IClientAuthService
    {
        private readonly IClientUpsertRepository _clientUpsertRepository;
        private readonly IPasswordService _passwordService;

        public ClientAuthService(IClientRetrievalRepository clientRetrievalRepository,
            IClientUpsertRepository clientUpsertRepository,
            IPasswordService passwordService) : base(clientRetrievalRepository)
        {
            _clientUpsertRepository = clientUpsertRepository;
            _passwordService = passwordService;
        }

        public async Task<Client> ClientLogin(string username, string password)
        {
            var client = await GetDbClientByUsername(username);

            await VerifyLoginPasswordAndIncrementFailedLoginAttemptsIfFail(password, client);

            if (client.IsLocked)
            {
                throw new ClientIsLockedException();
            }

            return await UpdateClientStatusAndReturnClient(client, true);
        }

        public async Task ClientLogout(long id)
        {
            var client = await GetDbClientById(id);

            if (client.IsLoggedIn == false)
            {
                return;
            }

            client.IsLoggedIn = false;

            await _clientUpsertRepository.UpdateClient(client);
        }

        private async Task IncrementClientFailedLoginAttempts(ClientDB client)
        {
            if (client.IsLocked)
            {
                return;
            }

            var attempts = await _clientUpsertRepository.IncrementClientFailedLoginAttempts(client.Id);

            if (attempts != 3)
            {
                return;
            }

            client.IsLocked = true;
            client.FailedLoginAttempts = attempts;

            await _clientUpsertRepository.UpdateClient(client);
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

        private async Task<Client> UpdateClientStatusAndReturnClient(ClientDB client, bool clientStatus)
        {
            if (client.IsLoggedIn != clientStatus)
            {
                client.IsLoggedIn = clientStatus;

                await _clientUpsertRepository.UpdateClient(client);
            }

            return ClientMapper.ToCoreClient(client);
        }
    }
}
