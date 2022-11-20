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

    public class PetServiceRepository : BaseRepository, IPetServiceRepository
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

                var totalPages = base.GetTotalPages(petServices.Count(), offset);

                //not more pet services
                if (page > totalPages)
                {
                    return (new List<PetServices>(), totalPages);
                }

                var skip = (page - 1) * offset;
                var result = await petServices.OrderByDescending(p => p.Id).Skip(skip).Take(offset).ToListAsync();

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
            return await base.GetEntityById<PetServices>(id);
        }

        /// <summary>
        /// Adds a new pet service.        
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<short> AddPetService(PetServices service)
        {
            return (await base.CreateEntity(service)).Id;
        }

        /// <summary>
        /// Updates a pet service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task UpdatePetService(PetServices service)
        {
            await base.UpdateEntity(service);
        }

        /// <summary>
        /// Removes a pet service.
        /// 
        /// Due to foreign key dependency, need to remove holiday rate tied to pet service first.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task DeletePetService(short petServiceId)
        {
            using (var context = new RofSchedulerContext())
            {
                var holidayRates = await context.HolidayRates.Where(r => r.PetServiceId == petServiceId).ToListAsync();

                if (holidayRates.Count > 0)
                {
                    //foreign key - need to remove all holiday rates tied to this pet service
                    context.HolidayRates.RemoveRange(holidayRates);
                }
              
                var petService = await context.PetServices.FirstOrDefaultAsync(p => p.Id == petServiceId);

                if (petService == null)
                {
                    return;
                }

                context.PetServices.Remove(petService);

                await context.SaveChangesAsync();
            }
        }
    }
}
