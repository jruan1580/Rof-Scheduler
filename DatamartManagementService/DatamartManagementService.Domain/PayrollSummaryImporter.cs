using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class PayrollSummaryImporter : DataImportHelper
    {
        private readonly IPayrollUpsertRepository _payrollSummaryUpsertRepo;

        public PayrollSummaryImporter(IRofSchedRepo rofSchedRepo, 
            IPayrollUpsertRepository payrollSummaryUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo) 
        : base(rofSchedRepo, jobExecutionHistoryRepo)
        {
            _payrollSummaryUpsertRepo = payrollSummaryUpsertRepo;
        }

        public async Task ImportPayrollSummary()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("payroll summary");

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, DateTime.Today);

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private async Task<List<EmployeePayroll>> GetListOfEmployeePayroll(List<JobEvent> completedEvents)
        {

        }

        private async Task<EmployeePayroll> GetEmployeePayroll(JobEvent jobEvent)
        {

        }

        private async Task<List<long>> GetUniqueEmployeeIdsFromCompletedEvents(List<JobEvent> completedEvents)
        {

        }

        private async Task<List<PetServices>> GetPetServiceInfo(List<JobEvent> jobEvents)
        {
            var petServiceInfo = new List<PetServices>();

            foreach (var job in jobEvents)
            {
                var dbService = await _rofSchedRepo.GetPetServiceById(job.PetServiceId);

                var petService = RofSchedulerMappers.ToCorePetService(dbService);

                var isHolidayRate = await CheckIfHolidayRate(job.EventEndTime);

                if (isHolidayRate)
                {
                    await UpdateToHolidayPayRate(petService);
                }

                petServiceInfo.Add(petService);
            }

            return petServiceInfo;
        }

        private decimal CalculateEmployeeTotalPay()
        {

        }

        //public long Id { get; set; }
        //public long EmployeeId { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public decimal EmployeeTotalPay { get; set; }
        //public DateTime PayPeriodStartDate { get; set; }
        //public DateTime PayPeriodEndDate { get; set; }
    }
}
