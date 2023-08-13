using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities
{
    public partial class RofRevenueFromServicesCompletedByDate
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
