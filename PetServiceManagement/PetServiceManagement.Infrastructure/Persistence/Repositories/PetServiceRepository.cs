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
        Task<(List<PetServices>, int)> GetAllPetServicesByPageAndKeyword(int page, int offset, string keyword = null);
        Task<List<PetServices>> GetAllPetServicesForDropdown();
        Task<PetServices> GetPetServiceById(short id);
        Task UpdatePetService(PetServices service);
        Task DeletePetService(short petServiceId);
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
        public async Task<(List<PetServices>, int)> GetAllPetServicesByPageAndKeyword(int page, int offset, string keyword = null)
        {
            using (var context = new RofSchedulerContext())
            {
                IQueryable<PetServices> petServices = null;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();

                    petServices = context.PetServices.Where(p =>
                        p.ServiceName.ToLower().Contains(keyword)
                        || p.Description.ToLower().Contains(keyword)
                     );
                }
                else
                {
                    petServices = context.PetServices.AsQueryable();
                }

                var fullCount = petServices.Count();
                var fullPages = fullCount / offset; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
                var remaining = fullCount % offset; //remaining will be 3 which will be an extra page
                var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

                //not more pet services
                if (page > totalPages)
                {
                    return (new List<PetServices>(), totalPages);
                }

                var skip = (page - 1) * offset;
                var result = await petServices.OrderByDescending(p => p.Id).Skip(skip).Take(10).ToListAsync();

                return (result, totalPages);
            }
        }

        /// <summary>
        /// gets a list of all pet services for dropdown purposes
        /// </summary>
        /// <returns></returns>
        public async Task<List<PetServices>> GetAllPetServicesForDropdown()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.PetServices.ToListAsync();
            }
        }

        /// <summary>
        /// When we update, we get the current original version of pet service to do a merge.
        /// This method is used to support that
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PetServices> GetPetServiceById(short id)
        {
            using(var context = new RofSchedulerContext())
            {
                return await context.PetServices.FirstOrDefaultAsync(p => p.Id == id);
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

        /// <summary>
        /// Removes a pet service.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task DeletePetService(short petServiceId)
        {
            using (var context = new RofSchedulerContext())
            {
                var service = await context.PetServices.FirstOrDefaultAsync(p => p.Id == petServiceId);

                context.PetServices.Remove(service);

                await context.SaveChangesAsync();
            }
        }
    }
}
