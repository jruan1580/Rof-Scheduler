using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRepository
    {
        Task CreateClient(Client newClient);
        Task DeleteClientById(long id);
        Task<short> IncrementClientFailedLoginAttempts(long id);
        Task UpdateClient(Client clientToUpdate);
    }

    public class ClientRepository : IClientRepository
    {
        public async Task CreateClient(Client newClient)
        {
            using (var context = new RofSchedulerContext())
            {
                var usa = context.Countries.FirstOrDefault(c => c.Name.Equals("United States of America"));

                if (usa == null)
                {
                    throw new Exception("Unable to find country United States of America");
                }

                newClient.CountryId = usa.Id;
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

        public async Task<short> IncrementClientFailedLoginAttempts(long id)
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

        public async Task UpdateClient(Client clientToUpdate)
        {
            using (var context = new RofSchedulerContext())
            {
                context.Clients.Update(clientToUpdate);

                await context.SaveChangesAsync();
            }
        }
    }
}
