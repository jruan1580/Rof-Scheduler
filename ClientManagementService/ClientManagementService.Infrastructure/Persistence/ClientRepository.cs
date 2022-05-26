using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRepository
    {
        Task CreateClient(Client newClient);
        Task DeleteClientById(long id);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);
        Task<Client> GetClientByUsername(string username);
        Task<int> IncrementClientFailedLoginAttempts(long id);
        Task ResetClientFailedLoginAttempts(long id);
        Task UpdateClientInfo(Client clientToUpdate);
        Task UpdateClientIsLocked(long id, bool isLocked);
        Task UpdateClientLoginStatus(long id, bool isLoggedIn);
        Task UpdatePassword(long id, byte[] newPassword);
        Task<bool> ClientAlreadyExists(long id, string email, string firstName, string lastName, string username);
    }

    public class ClientRepository : IClientRepository
    {
        public async Task CreateClient(Client newClient)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Clients.Add(newClient);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteClientById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if(client == null)
                {
                    throw new ArgumentException($"No client with Id: {id} found.");
                }

                context.Remove(client);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Client>> GetAllClient()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.ToListAsync();
            }
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.FirstOrDefaultAsync(c => c.EmailAddress == email);
            }
        }

        public async Task<Client> GetClientById(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            }
        }

        public async Task<Client> GetClientByUsername(string username)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.FirstOrDefaultAsync(c => c.Username == username);
            }
        }

        public async Task<int> IncrementClientFailedLoginAttempts(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if (client == null)
                {
                    throw new ArgumentException("Client not found.");
                }

                client.FailedLoginAttempts += 1;

                await context.SaveChangesAsync();

                return client.FailedLoginAttempts;
            }
        }

        public async Task ResetClientFailedLoginAttempts(long id)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if (client == null)
                {
                    throw new ArgumentException("Client not found.");
                }

                client.FailedLoginAttempts = 0;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateClientInfo(Client clientToUpdate)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Clients.Update(clientToUpdate);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateClientIsLocked(long id, bool isLocked)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if (client == null)
                {
                    throw new ArgumentException("Client not found.");
                }

                client.IsLocked = isLocked;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateClientLoginStatus(long id, bool isLoggedIn)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if (client == null)
                {
                    throw new ArgumentException("Client not found.");
                }

                client.IsLoggedIn = isLoggedIn;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePassword(long id, byte[] newPassword)
        {
            using (var context = new RofSchedulerContext())
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);

                if (client == null)
                {
                    throw new ArgumentException("No client found.");
                }

                client.Password = newPassword;

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ClientAlreadyExists(long id, string email, string firstName, string lastName, string username)
        {
            using (var context = new RofSchedulerContext())
            {
                email = email.ToLower();
                firstName = firstName.ToLower();
                lastName = lastName.ToLower();
                username = username.ToLower();

                return await context.Clients.AnyAsync(c => c.Id != id &&
                    ((c.EmailAddress.ToLower().Equals(email) && c.FirstName.ToLower().Equals(firstName) && c.LastName.ToLower().Equals(lastName)) 
                        || c.Username.ToLower().Equals(username)));                
            }
        }
    }
}
