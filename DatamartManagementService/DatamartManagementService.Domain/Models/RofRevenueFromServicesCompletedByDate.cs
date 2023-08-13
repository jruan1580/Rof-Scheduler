using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models
{
    public class RofRevenueFromServicesCompletedByDate
    {
        public long EmployeeId { get; set; }
        
        public string EmployeeFirstName { get; set; }
        
        public string EmployeeLastName { get; set; }
        
        public decimal EmployeePay { get; set; }
        
        public short PetServiceId { get; set; }
        
        public string PetServiceName { get; set; }
        
        public decimal PetServiceRate { get; set; }
        
        public bool IsHolidayRate { get; set; }
        
        public decimal NetRevenuePostEmployeeCut { get; set; }
        
        public DateTime RevenueDate { get; set; }
    }
}
