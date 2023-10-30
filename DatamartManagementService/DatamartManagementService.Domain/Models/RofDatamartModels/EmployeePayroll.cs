using System;

namespace DatamartManagementService.Domain.Models.RofDatamartModels
{
    public class EmployeePayroll
    {
        public long Id { get; set; }

        public long EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal EmployeeTotalPay { get; set; }

        public DateTime PayrollDate { get; set; }

        public short PayrollMonth { get; set; }

        public short PayrollYear { get; set; }
    }
}
