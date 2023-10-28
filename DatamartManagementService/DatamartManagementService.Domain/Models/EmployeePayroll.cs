using System;

namespace DatamartManagementService.Domain.Models
{
    public class EmployeePayroll
    {
        public long Id { get; set; }

        public long EmployeeId { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public decimal EmployeeTotalPay { get; set; }

        public DateTime RevenueDate { get; set; }

        public short RevenueMonth { get; set; }
        
        public short RevenueYear { get; set; }
    }
}
