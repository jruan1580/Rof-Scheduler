﻿using DatamartManagementService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence
{
    public interface IRevenueByDateRetrievalRepository
    {
        Task<List<RofRevenueByDate>> GetRevenueForTheMonthOfCertainYear(short month, short year);
        Task<List<RofRevenueByDate>> GetRevenueForTheYear(short year);
    }

    public class RevenueByDateRetrievalRepository : IRevenueByDateRetrievalRepository
    {
        public async Task<List<RofRevenueByDate>> GetRevenueForTheYear(short year)
        {
            using var context = new RofDatamartContext();

            var yearlyRevenue = await context.RofRevenueByDate.Where(r => r.RevenueYear == year).ToListAsync();

            return yearlyRevenue;
        }

        public async Task<List<RofRevenueByDate>> GetRevenueForTheMonthOfCertainYear(short month, short year)
        {
            using var context = new RofDatamartContext();

            var monthlyRevenue = await context.RofRevenueByDate.Where(r => r.RevenueMonth == month && r.RevenueYear == year).ToListAsync();

            return monthlyRevenue;
        }
    }
}
