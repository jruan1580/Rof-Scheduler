using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.EntityFrameworkCore;
using RofShared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.Infrastructure.Persistence
{
    public interface IClientRetrievalRepository
    {
        Task<bool> DoesClientExistByEmailOrUsername(long id, string email, string username);
        Task<(List<Client>, int)> GetAllClientsByKeyword(int page = 1, int offset = 10, string keyword = "");
        Task<Client> GetClientByFilter<T>(GetClientFilterModel<T> filter);
        Task<List<Client>> GetClientsForDropdown();
    }

    public class ClientRetrievalRepository : IClientRetrievalRepository
    {
        public async Task<List<Client>> GetClientsForDropdown()
        {
            using var context = new RofSchedulerContext();

            return await context.Clients.ToListAsync();
        }

        public async Task<(List<Client>, int)> GetAllClientsByKeyword(int page = 1, int offset = 10, string keyword = "")
        {
            using var context = new RofSchedulerContext();

            var skip = (page - 1) * offset;
            var clients = FilterByKeyword(context, keyword?.Trim()?.ToLower());

            var countByCriteria = await clients.CountAsync();

            var totalPages = DatabaseUtilities.GetTotalPages(countByCriteria, offset, page);

            var result = await clients.OrderByDescending(e => e.Id).Skip(skip).Take(offset).ToListAsync();

            return (result, totalPages);
        }

        public async Task<Client> GetClientByFilter<T>(GetClientFilterModel<T> filter)
        {
            using var context = new RofSchedulerContext();

            if (filter.Filter == GetClientFilterEnum.Id)
            {
                var val = Convert.ToInt64(filter.Value);

                return await context.Clients.FirstOrDefaultAsync(c => c.Id == val);
            }
            else if (filter.Filter == GetClientFilterEnum.Username)
            {
                var username = Convert.ToString(filter.Value).ToLower();

                return await context.Clients.FirstOrDefaultAsync(e => e.Username.ToLower().Equals(username));
            }
            else if (filter.Filter == GetClientFilterEnum.Email)
            {
                var email = Convert.ToString(filter.Value).ToLower();

                return await context.Clients.FirstOrDefaultAsync(c => c.EmailAddress.ToLower().Equals(email));
            }
            else
            {
                throw new ArgumentException("Invalid Filter Type.");
            }
        }

        public async Task<bool> DoesClientExistByEmailOrUsername(long id, string email, string username)
        {
            using var context = new RofSchedulerContext();

            return await context.Clients.AnyAsync(c => c.Id != id
            && (c.EmailAddress.ToLower().Equals(email.ToLower())
            || c.Username.ToLower().Equals(username.ToLower())));
        }

        private IQueryable<Client> FilterByKeyword(RofSchedulerContext context, string keyword)
        {
            var clients = context.Clients.AsQueryable();

            if (string.IsNullOrEmpty(keyword))
            {
                return clients;
            }

            return clients.Where(e => (e.FirstName.ToLower().Contains(keyword))
                || (e.LastName.ToLower().Contains(keyword))
                || (e.EmailAddress.ToLower().Contains(keyword)));
        }
    }
}
