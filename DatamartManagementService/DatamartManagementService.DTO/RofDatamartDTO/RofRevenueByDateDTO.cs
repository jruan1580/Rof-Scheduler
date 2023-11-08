using System;

namespace DatamartManagementService.DTO.RofDatamartDTO
{
    public class RofRevenueByDateDTO
    {
        public long Id { get; set; }

        public short PetServiceId { get; set; }

        public DateTime RevenueDate { get; set; }

        public short RevenueMonth { get; set; }

        public short RevenueYear { get; set; }

        public decimal GrossRevenue { get; set; }

        public decimal NetRevenuePostEmployeePay { get; set; }
    }
}
