using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using RofShared.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IPetServiceRetrievalRepository
    {
        Task<(List<PetServices>, int)> GetAllPetServicesByPageAndKeyword(int page, int offset, string keyword = null);
        Task<List<PetServices>> GetAllPetServicesForDropdown();
        Task<PetServices> GetPetServiceById(short id);
    }

    public class PetServiceRetrievalRepository : BaseRepository, IPetServiceRetrievalRepository
    {
        public async Task<(List<PetServices>, int)> GetAllPetServicesByPageAndKeyword(int page, int offset, string keyword = null)
        {
            using var context = new RofSchedulerContext();

            var petServices = FilterByKeyword(context, keyword?.Trim()?.ToLower());

            var totalPages = DatabaseUtilities.GetTotalPages(petServices.Count(), offset, page);

            //not more pet services
            if (page > totalPages)
            {
                return (new List<PetServices>(), totalPages);
            }

            var skip = (page - 1) * offset;
            var result = await SkipNAndTakeTopM(petServices, skip, 10);

            return (result, totalPages);
        }

        public async Task<List<PetServices>> GetAllPetServicesForDropdown()
        {
            using var context = new RofSchedulerContext();

            return await context.PetServices.ToListAsync();
        }

        public async Task<PetServices> GetPetServiceById(short id)
        {
            return await base.GetEntityById<PetServices>(id);
        }

        private IQueryable<PetServices> FilterByKeyword(RofSchedulerContext context, string keyword)
        {
            var petServices = context.PetServices.AsQueryable();
            if (string.IsNullOrEmpty(keyword))
            {
                return petServices.OrderByDescending(p => p.Id);
            }

            return context.PetServices
                .Where(p => p.ServiceName.ToLower().Contains(keyword)
                    || p.Description.ToLower().Contains(keyword))
                .OrderByDescending(p => p.Id);
        }
    }
}
