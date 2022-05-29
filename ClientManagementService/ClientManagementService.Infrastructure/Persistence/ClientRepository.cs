using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters;
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
        Task<List<Client>> GetAllClients();
        Task<Client> GetClientByFilter<T>(GetClientFilterModel<T> filter);
        Task<short> IncrementClientFailedLoginAttempts(long id);
        Task UpdateClient(Client clientToUpdate);
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

        public async Task<List<Client>> GetAllClients()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.ToListAsync();
            }
        }

        public async Task<Client> GetClientByFilter<T>(GetClientFilterModel<T> filter)
        {
            using (var context = new RofSchedulerContext())
            {
                if(filter.Filter == GetClientFilterEnum.Id)
                {
                    return await context.Clients.FirstOrDefaultAsync(c => c.Id == Convert.ToInt64(filter.Value));
                }
                else if(filter.Filter == GetClientFilterEnum.Username)
                {
                    return await context.Clients.FirstOrDefaultAsync(c => c.Username.ToLower().Equals(Convert.ToString(filter.Value).ToLower()));
                }
                else if(filter.Filter == GetClientFilterEnum.Email)
                {
                    return await context.Clients.FirstOrDefaultAsync(c => c.EmailAddress.ToLower().Equals(Convert.ToString(filter.Value).ToLower()));
                }
                else
                {
                    throw new ArgumentException("Invalid Filter Type.");
                }
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
