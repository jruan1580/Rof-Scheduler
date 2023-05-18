using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public class RevenueByDateRepository
    {
        public async Task AddRevenue(List<RofRevenueByDate> newRevenueByDate)
        {
            using var context = new RofDatamartContext();

            context.RofRevenueByDate.AddRange(newRevenueByDate);

            await context.SaveChangesAsync();
        }

        public async Task<List<RofRevenueByDate>> GetRevenueByYear(short year)
        {
            using var context = new RofDatamartContext();

            var yearlyRevenue = await context.RofRevenueByDate.Where(r => r.RevenueYear == year).ToListAsync();

            return yearlyRevenue;
        }
    }
}
