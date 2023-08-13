using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities
{
    public partial class RofRevenueByDate
    {
        public DateTime RevenueDate { get; set; }
        public short RevenueMonth { get; set; }
        public short RevenueYear { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal NetRevenuePostEmployeePay { get; set; }
    }
}
