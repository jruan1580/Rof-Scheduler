using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRepository
    {
        Task CreateClient(Client newClient);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);
        Task<int> IncrementClientFailedLoginAttempts(long id);
        Task ResetClientFailedLoginAttempts(long id);
        Task UpdateClientInfo(Client clientToUpdate);
        Task UpdateClientIsLocked(long id, bool isLocked);
        Task UpdateClientLoginStatus(long id, bool isLoggedIn);
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

        public async Task UpdateClientInfo(Client clientToUpdate)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Clients.Update(clientToUpdate);

                await context.SaveChangesAsync();
            }
        }

        public async Task<Client> GetClientById(long id)
        {
            //pet table not created cannot join for now...
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            }
        }

        public async Task<Client> GetClientByEmail(string email)
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.FirstOrDefaultAsync(c => c.EmailAddress == email);
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
    }
}
