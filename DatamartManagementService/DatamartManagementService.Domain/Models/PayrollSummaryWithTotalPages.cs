using System.Collections.Generic;

namespace DatamartManagementService.Domain.Models
{
    public class PayrollSummaryWithTotalPages
    {
        public PayrollSummaryWithTotalPages(List<PayrollSummaryPerEmployee> payrollSummaryPerEmployee, int totalPages)
        {
            PayrollSummaryPerEmployee = payrollSummaryPerEmployee;
            TotalPages = totalPages;
        }

        public List<PayrollSummaryPerEmployee> PayrollSummaryPerEmployee { get; set; }
        public int TotalPages { get; set; }
    }
}
