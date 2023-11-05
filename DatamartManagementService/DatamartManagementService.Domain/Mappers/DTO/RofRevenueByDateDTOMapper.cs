using DatamartManagementService.DTO.RofDatamartDTO;
using CoreRevenueSummary = DatamartManagementService.Domain.Models.RofDatamartModels.RofRevenueByDate;

namespace DatamartManagementService.Domain.Mappers.DTO
{
    public static class RofRevenueByDateDTOMapper
    {
        public static RofRevenueByDateDTO ToDTORevenueSummary(CoreRevenueSummary coreRevenueSummary)
        {
            var dtoRevenueSummary = new RofRevenueByDateDTO();

            dtoRevenueSummary.Id = coreRevenueSummary.Id;
            dtoRevenueSummary.PetServiceId = coreRevenueSummary.PetServiceId;
            dtoRevenueSummary.RevenueDate = coreRevenueSummary.RevenueDate;
            dtoRevenueSummary.RevenueMonth = coreRevenueSummary.RevenueMonth;
            dtoRevenueSummary.RevenueYear = coreRevenueSummary.RevenueYear;
            dtoRevenueSummary.GrossRevenue = coreRevenueSummary.GrossRevenue;
            dtoRevenueSummary.NetRevenuePostEmployeePay = coreRevenueSummary.NetRevenuePostEmployeePay;

            return dtoRevenueSummary;
        }

        public static CoreRevenueSummary FromDTORevenueSummary(RofRevenueByDateDTO dtoRevenueSummary)
        {
            var coreRevenueSummary = new CoreRevenueSummary();

            coreRevenueSummary.Id = dtoRevenueSummary.Id;
            coreRevenueSummary.PetServiceId = dtoRevenueSummary.PetServiceId;
            coreRevenueSummary.RevenueDate = dtoRevenueSummary.RevenueDate;
            coreRevenueSummary.RevenueMonth = dtoRevenueSummary.RevenueMonth;
            coreRevenueSummary.RevenueYear = dtoRevenueSummary.RevenueYear;
            coreRevenueSummary.GrossRevenue = dtoRevenueSummary.GrossRevenue;
            coreRevenueSummary.NetRevenuePostEmployeePay = dtoRevenueSummary.NetRevenuePostEmployeePay;

            return coreRevenueSummary;
        }
    }
}
