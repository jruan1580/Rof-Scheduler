using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IPetServiceRepository
    {
        Task<short> AddPetService(PetServices service);
        Task<(List<PetServices>, int)> GetAllPetServices(int page, int offset, string keyword = null);
        Task UpdatePetService(PetServices service);
    }

    public class PetServiceRepository : IPetServiceRepository
    {
        /// <summary>
        /// Displays a list of pet services base on the page.
        /// Offset is how many to display per page.
        /// Keyword is what to filter by if passed in
        /// </summary>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<(List<PetServices>, int)> GetAllPetServices(int page, int offset, string keyword = null)
        {
            using (var context = new RofSchedulerContext())
            {
                List<PetServices> petServices = null;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();

                    petServices = await context.PetServices.Where(p =>
                        p.ServiceName.ToLower().Contains(keyword)
                        || p.Description.ToLower().Contains(keyword)
                     ).ToListAsync();
                }
                else
                {
                    petServices = await context.PetServices.ToListAsync();
                }

                var fullCount = petServices.Count;
                var fullPages = fullCount / offset; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
                var remaining = fullCount % offset; //remaining will be 3 which will be an extra page
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

                //not more pets
                if (page > totalPages)
                {
                    return (new List<PetServices>(), totalPages);
                }

                var skip = (page - 1) * offset;
                var result = petServices.OrderByDescending(p => p.Id).Skip(skip).Take(10).ToList();

                return (result, totalPages);
            }
        }

        /// <summary>
        /// Adds a new pet service.        
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<short> AddPetService(PetServices service)
        {
            using (var context = new RofSchedulerContext())
            {
                context.PetServices.Add(service);

                await context.SaveChangesAsync();

                return service.Id;
            }
        }

        /// <summary>
        /// Updates a pet service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task UpdatePetService(PetServices service)
        {
            using (var context = new RofSchedulerContext())
            {
                context.PetServices.Update(service);

                await context.SaveChangesAsync();
            }
        }
    }
}
