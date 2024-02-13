using DatamartManagementService.Domain.Models;
using DatamartManagementService.DTO;

namespace DatamartManagementService.Domain.Mappers.DTO
{
    public static class PayrollSummaryPerEmployeeDTOMapper
    {
        public static PayrollSummaryPerEmployeeDTO ToDTOPayrollSummaryPerEmployee(PayrollSummaryPerEmployee payrollSummaryPerEmployee)
        {
            var dtoPayroll = new PayrollSummaryPerEmployeeDTO();

            payrollSummaryPerEmployee.FirstName = dtoPayroll.FirstName;
            payrollSummaryPerEmployee.LastName = dtoPayroll.LastName;
            payrollSummaryPerEmployee.TotalPay = dtoPayroll.TotalPay;

            return dtoPayroll;
        }
    }
}
