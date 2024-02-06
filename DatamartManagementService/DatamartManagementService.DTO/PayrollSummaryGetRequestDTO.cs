using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.DTO
{
    public class PayrollSummaryGetRequestDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int Page { get; set; }
    }
}
