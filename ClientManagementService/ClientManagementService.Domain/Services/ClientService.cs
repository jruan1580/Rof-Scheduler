using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public class ClientService
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

            if(invalidErrs.Length > 0)
            {
                var errMsg = string.Join("\n", invalidErrs);

                throw new ArgumentException(errMsg);
            }

            //check client email and name unique

            if (!_passwordService.VerifyPasswordRequirements(password))
            {
                throw new ArgumentException("Password does not meet requirements");
            }

            var encryptedPass = _passwordService.EncryptPassword(password);
            newClient.Password = encryptedPass;

            var newClientEntity = ClientMapper.FromCoreClient(newClient);

            await _clientRepository.CreateClient(newClientEntity);
        }
    }
}
