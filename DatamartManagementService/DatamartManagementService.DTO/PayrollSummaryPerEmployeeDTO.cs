using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.DTO
{
    public class PayrollSummaryPerEmployeeDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal TotalPay { get; set; }
    }
}
