using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public class CountryRepository
    {
        public async Task<List<Country>> GetAllCountries()
        {
            using (var context = new RofSchedulerContext())
            {
                return await context.Countries.ToListAsync();
            }
        }
    }
}
