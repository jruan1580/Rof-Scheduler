using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Test
{
    public static class ModelCreator
    {
        public static RevenueSummaryPerPetService GetCoreRevenueSummaryPerPetService()
        {
            return new RevenueSummaryPerPetService(GetCorePetServices(), 1, 25, 10);
        }

        public static PetServices GetCorePetServices()
        {
            return new PetServices()
            {
                Id = 1,
                ServiceName = "Walking"
            };
        }

        public static PayrollSummaryPerEmployee GetCorePayrollSummaryPerEmployee()
        {
            return new PayrollSummaryPerEmployee("John", "Doe", 20);
        }

        public static PayrollSummaryWithTotalPages GetCorePayrollSummaryWithTotalPages()
        {
            var payrollSummaryPerEmployee = new List<PayrollSummaryPerEmployee>()
            {
                GetCorePayrollSummaryPerEmployee()
            };

            return new PayrollSummaryWithTotalPages(payrollSummaryPerEmployee, 1);
        }
    }
}
