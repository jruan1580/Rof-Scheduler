using System;

namespace DatamartManagementService.Domain.Models.RofDatamartModels
{
    public class RofRevenueByDate
    {
        public long Id { get; set; }

        public DateTime RevenueDate { get; set; }

        public short RevenueMonth { get; set; }

        public short RevenueYear { get; set; }

        public decimal GrossRevenue { get; set; }

        public decimal NetRevenuePostEmployeePay { get; set; }
    }
}
