using System.Collections.Generic;

namespace DatamartManagementService.DTO
{
    public class PayrollSummaryWithTotalPagesDTO
    {
        public PayrollSummaryWithTotalPagesDTO(List<PayrollSummaryPerEmployeeDTO> payrollSummaryPerEmployeeDTO, int totalPages)
        {
            PayrollSummaryPerEmployeeDTO = payrollSummaryPerEmployeeDTO;
            TotalPages = totalPages;
        }

        public List<PayrollSummaryPerEmployeeDTO> PayrollSummaryPerEmployeeDTO { get; set; }

        public int TotalPages { get; set; }
    }
}
