using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRepository
    {
        Task CreateClient(Client newClient);
        Task DeleteClientById(long id);
        Task<List<Client>> GetClientsForDropdown();
        Task<(List<Client>, int)> GetAllClientsByKeyword(int page = 1, int offset = 10, string keyword = "");
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

        public async Task<List<Client>> GetClientsForDropdown()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Clients.Select(c => new Client() { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName}).ToListAsync();
            }
        }

        public async Task<(List<Client>, int)> GetAllClientsByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using (var context = new RofSchedulerContext())
            {
                var skip = (page - 1) * offset;
                IQueryable<Client> client = context.Clients;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();

                    client = context.Clients
                        .Where(e => (e.FirstName.ToLower().Contains(keyword))
                            || (e.LastName.ToLower().Contains(keyword))
                            || (e.EmailAddress.ToLower().Contains(keyword)));
                }

                var countByCriteria = await client.CountAsync();

                if(countByCriteria == 0)
                {
                    return (new List<Client>(), 0);
                }

                var fullPages = countByCriteria / offset; 
                var remaining = countByCriteria % offset; 
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; 

                if (page > totalPages)
                {
                    throw new ArgumentException("No more employees.");
                }

                var result = await client.OrderByDescending(e => e.Id).Skip(skip).Take(offset).ToListAsync();

                return (result, totalPages);
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
