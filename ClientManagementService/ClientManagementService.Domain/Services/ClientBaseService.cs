using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using RofShared.Exceptions;
using System.Threading.Tasks;
using ClientDb = ClientManagementService.Infrastructure.Persistence.Entities.Client;


namespace ClientManagementService.Domain.Services
{
    public class ClientBaseService
    {
        protected readonly IClientRetrievalRepository _clientRetrievalRepository;

        public ClientBaseService(IClientRetrievalRepository clientRetrievalRepository)
        {
            _clientRetrievalRepository = clientRetrievalRepository;
        }

        protected async Task<ClientDb> GetDbClientById(long id)
        {
            var filterModel = new GetClientFilterModel<long>(GetClientFilterEnum.Id, id);

            var client = await _clientRetrievalRepository.GetClientByFilter(filterModel);

            if (client == null)
            {
                throw new EntityNotFoundException("Client");
            }

            return client;
        }

        protected async Task<ClientDb> GetDbClientByUsername(string username)
        {
            var filterModel = new GetClientFilterModel<string>(GetClientFilterEnum.Username, username);

            var client = await _clientRetrievalRepository.GetClientByFilter(filterModel);

            if (client == null)
            {
                throw new EntityNotFoundException("Client");
            }

            return client;
        }

        protected async Task<ClientDb> GetDbClientByEmail(string email)
        {
            var filterModel = new GetClientFilterModel<string>(GetClientFilterEnum.Email, email);

            var client = await _clientRetrievalRepository.GetClientByFilter(filterModel);

            if (client == null)
            {
                throw new EntityNotFoundException("Client");
            }

            return client;
        }
    }
}
