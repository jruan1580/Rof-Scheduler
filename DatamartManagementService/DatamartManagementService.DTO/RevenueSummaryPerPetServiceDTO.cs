using DatamartManagementService.DTO.RofSchedulerDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.DTO
{
    public class RevenueSummaryPerPetServiceDTO
    {
        public RevenueSummaryPerPetServiceDTO(PetServicesDTO petService, int count,
            decimal grossRevenue, decimal netRevenue)
        {
            PetService = petService;
            Count = count;
            GrossRevenuePerService = grossRevenue;
            NetRevenuePerService = netRevenue;
        }

        public PetServicesDTO PetService { get; set; }
        public int Count { get; set; }
        public decimal GrossRevenuePerService { get; set; }
        public decimal NetRevenuePerService { get; set; }
    }
}
