﻿using ClientManagementService.Domain.Mappers.Database;
using ClientManagementService.Domain.Models;
using ClientManagementService.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IClientService
    {
        Task CreateClient(Client newClient, string password);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);
        Task UpdateClientInfo(Client client);
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

            var clientCheck = await GetClientByEmail(newClient.EmailAddress);

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
    }
}
