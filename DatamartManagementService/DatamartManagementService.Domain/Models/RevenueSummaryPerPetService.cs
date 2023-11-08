using DatamartManagementService.Domain.Models.RofSchedulerModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models
{
    public class RevenueSummaryPerPetService
    {
        public RevenueSummaryPerPetService(PetServices petService, int count, 
            decimal grossRevenue, decimal netRevenue)
        {
            PetService = petService;
            Count = count;
            GrossRevenuePerService = grossRevenue;
            NetRevenuePerService = netRevenue;
        }

        public PetServices PetService { get; set; }
        public int Count { get; set; }
        public decimal GrossRevenuePerService { get; set; }
        public decimal NetRevenuePerService { get; set; }
    }
}
