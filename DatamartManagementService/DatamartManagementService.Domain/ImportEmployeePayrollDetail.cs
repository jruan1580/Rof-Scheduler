using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportEmployeePayrollDetail: AImportRevenuePayroll
    {
        private readonly IRofSchedRepo _rofSchedRepo;

        public async Task<List<EmployeePayrollDetail>> PopulateListOfEmployeePayrollDetails(List<JobEvent> jobEvents)
        {
            var employeePayrollDetails = new List<EmployeePayrollDetail>();

            foreach (var job in jobEvents)
            {
                var payrollDetail = await PopulateEmployeePayrollDetail(job.EmployeeId, job.PetServiceId, job.Id);
                employeePayrollDetails.Add(payrollDetail);
            }

            return employeePayrollDetails;
        }

        public async Task<EmployeePayrollDetail> PopulateEmployeePayrollDetail(long employeeId, short petServiceId, int jobEventId)
        {
            var employeeInfo = await _rofSchedRepo.GetEmployeeById(employeeId);
            var petService = await _rofSchedRepo.GetPetServiceById(petServiceId);
            var jobEvent = await _rofSchedRepo.GetJobEventById(jobEventId);

            var isHoliday = await CheckIfEventIsHoliday(jobEvent.EventStartTime);

            var employeePayrollDetail = new EmployeePayrollDetail()
            {
                EmployeeId = employeeInfo.Id,
                FirstName = employeeInfo.FirstName,
                LastName = employeeInfo.LastName,
                PetServiceId = petService.Id,
                PetServiceName = petService.ServiceName,
                EmployeePayForService = petService.EmployeeRate,
                ServiceDuration = petService.Duration,
                ServiceDurationTimeUnit = petService.TimeUnit,
                JobEventId = jobEventId,
                IsHolidayPay = isHoliday,
                ServiceStartDateTime = jobEvent.EventStartTime,
                ServiceEndDateTime = jobEvent.EventEndTime
            };

            return employeePayrollDetail;
        }
    }
}
