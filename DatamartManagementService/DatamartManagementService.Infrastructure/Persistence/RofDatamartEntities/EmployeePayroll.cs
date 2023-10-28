using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities
{
    public partial class EmployeePayroll
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
