using DatamartManagementService.Domain.Models;
using DatamartManagementService.DTO;

namespace DatamartManagementService.Domain.Mappers.DTO
{
    public static class PayrollSummaryPerEmployeeDTOMapper
    {
        public static PayrollSummaryPerEmployeeDTO ToDTOPayrollSummaryPerEmployee(PayrollSummaryPerEmployee payrollSummaryPerEmployee)
        {
            var dtoPayroll = new PayrollSummaryPerEmployeeDTO();

            dtoPayroll.FirstName = payrollSummaryPerEmployee.FirstName;
            dtoPayroll.LastName = payrollSummaryPerEmployee.LastName;
            dtoPayroll.TotalPay = payrollSummaryPerEmployee.TotalPay;

            return dtoPayroll;
        }
    }
}
