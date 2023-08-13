using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models
{
    public class RofRevenueByDate
    {
        public DateTime RevenueDate { get; set; }
        
        public short RevenueMonth { get; set; }
        
        public short RevenueYear { get; set; }
        
        public decimal GrossRevenue { get; set; }
        
        public decimal NetRevenuePostEmployeePay { get; set; }
    }
}
