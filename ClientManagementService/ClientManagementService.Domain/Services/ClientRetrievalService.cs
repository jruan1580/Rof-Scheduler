using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public class ClientRetrievalService : ClientService, IClientRetrievalService
    {
        public ClientRetrievalService(IClientRetrievalRepository clientRetrievalRepository)
            : base(clientRetrievalRepository) { }

        public async Task<ClientsWithTotalPage> GetAllClientsByKeyword(int page, int offset, string keyword)
        {
            var result = await _clientRetrievalRepository.GetAllClientsByKeyword(page, offset, keyword);

            var clients = result.Item1;
            var totalPages = result.Item2;

            var coreClients = clients.Select(c => ClientMapper.ToCoreClient(c)).ToList();

            return new ClientsWithTotalPage(coreClients, totalPages);
        }

        public async Task<Client> GetClientById(long id)
        {
            var client = await GetDbClientById(id);

            return ClientMapper.ToCoreClient(client);
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            var client = await GetDbClientByEmail(email);

            return ClientMapper.ToCoreClient(client);
        }

        public async Task<Client> GetClientByUsername(string username)
        {
            var client = await GetDbClientByUsername(username);

            return ClientMapper.ToCoreClient(client);
        }
    }
}
