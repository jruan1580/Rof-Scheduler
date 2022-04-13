using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRepository
    {
        Task CreateClient(Client newClient);
        Task<Client> GetClientById(long id);
        Task UpdateClientInfo(Client clientToUpdate);
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
    }
}
