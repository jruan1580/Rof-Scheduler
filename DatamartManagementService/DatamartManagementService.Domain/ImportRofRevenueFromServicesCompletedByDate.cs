using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRofRevenueFromServicesCompletedByDate : AImportRevenuePayroll
    {
        private readonly IRofSchedRepo _rofSchedRepo;

        public async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(List<EmployeePayrollDetail> employees)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach (var employee in employees)
            {
                var revenue = await PopulateRofRevenueForServicesCompletedByDate(employee);

                revenueForServiceCompleted.Add(revenue);
            }

            return revenueForServiceCompleted;
        }

        public async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(EmployeePayrollDetail employeePayrollDetail)
        {
            var petService = await _rofSchedRepo.GetPetServiceById(employeePayrollDetail.PetServiceId);

            var netRevenue = await CalculateNetRevenueEarnedByDate(employeePayrollDetail.EmployeeId,
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeePayrollDetail.EmployeeId,
                EmployeeFirstName = employeePayrollDetail.FirstName,
                EmployeeLastName = employeePayrollDetail.LastName,
                EmployeePay = employeePayrollDetail.EmployeePayForService,
                PetServiceId = employeePayrollDetail.PetServiceId,
                PetServiceName = employeePayrollDetail.PetServiceName,
                PetServiceRate = petService.EmployeeRate,
                IsHolidayRate = employeePayrollDetail.IsHolidayPay,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = employeePayrollDetail.ServiceEndDateTime
            };

            return rofRevenueForService;
        }
    }
}
