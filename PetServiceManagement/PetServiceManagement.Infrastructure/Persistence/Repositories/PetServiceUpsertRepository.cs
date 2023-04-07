using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public interface IPetServiceUpsertRepository
    {
        Task<short> AddPetService(PetServices service);
        Task DeletePetService(short petServiceId);
        Task UpdatePetService(PetServices service);
    }

    public class PetServiceUpsertRepository : BaseRepository, IPetServiceUpsertRepository
    {
        public async Task<short> AddPetService(PetServices service)
        {
            return (await base.CreateEntity(service)).Id;
        }

        public async Task UpdatePetService(PetServices service)
        {
            await base.UpdateEntity(service);
        }

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
