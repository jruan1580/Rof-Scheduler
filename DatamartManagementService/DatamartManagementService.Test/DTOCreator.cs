using DatamartManagementService.DTO;
using System;

namespace DatamartManagementService.Test
{
    public static class DTOCreator
    {
        public static PayrollSummaryGetRequestDTO GetDTOPayrollSummaryGetRequest()
        {
            return new PayrollSummaryGetRequestDTO()
            {
                FirstName = "John",
                LastName = "Doe",
                StartDate = DateTime.Today.AddDays(-1).ToString(),
                EndDate = DateTime.Today.ToString(),
            };
        }
    }
}
