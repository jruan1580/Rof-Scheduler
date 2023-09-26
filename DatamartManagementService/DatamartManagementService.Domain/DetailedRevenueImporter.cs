using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IDetailedRevenueImporter
    {
        Task ImportRevenueData();
    }

    public class DetailedRevenueImporter : DetailedDataImporter, IDetailedRevenueImporter
    {
        private readonly IRevenueFromServicesUpsertRepository _detailedRevenueUpsertRepo;

        public DetailedRevenueImporter(IRofSchedRepo rofSchedRepo,
            IRevenueFromServicesUpsertRepository detailedRevenueUpsertRepo,
            IJobExecutionHistoryRepository jobExecutionHistoryRepo)
        : base(rofSchedRepo, jobExecutionHistoryRepo)
        {
            _detailedRevenueUpsertRepo = detailedRevenueUpsertRepo;
        }

        public async Task ImportRevenueData()
        {
            try
            {
                var lastExecution = await GetJobExecutionHistory("revenue");

                var yesterday = DateTime.Today.AddDays(-1);

                var completedEvents = await GetCompletedJobEventsBetweenDate(lastExecution, yesterday);

                var listOfDetailedRofRev = await GetListOfRofRevenueOfCompletedServiceByDate(completedEvents);

                var revenueForServicesByDateDbEntity =
                    RofDatamartMappers.FromCoreRofRevenueFromServicesCompletedByDate(listOfDetailedRofRev);

                await _detailedRevenueUpsertRepo.AddRevenueFromServices(revenueForServicesByDateDbEntity);

                await AddJobExecutionHistory("Revenue", yesterday);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }           
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> GetListOfRofRevenueOfCompletedServiceByDate(
            List<JobEvent> completedEvents)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var completed in completedEvents)
            {
                var revenue = await GetRofRevenueForServicesCompletedByDate(completed);

                revenueForServiceCompleted.Add(revenue);
            }
            
            return revenueForServiceCompleted;
        }

        private async Task<RofRevenueFromServicesCompletedByDate> GetRofRevenueForServicesCompletedByDate(JobEvent jobEvent)
        {           
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(jobEvent.EmployeeId));

            var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(jobEvent.PetServiceId));

            var isHolidayRate = await CheckIfHolidayRate(jobEvent.EventEndTime);

            if (isHolidayRate)
            {
                await UpdateToHolidayPayRate(petServiceInfo);
            }

            var netRevenue = CalculateNetRevenueForCompletedService(petServiceInfo);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeeInfo.Id,
                EmployeeFirstName = employeeInfo.FirstName,
                EmployeeLastName = employeeInfo.LastName,
                EmployeePay = petServiceInfo.EmployeeRate,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                PetServiceRate = petServiceInfo.Price,
                IsHolidayRate = isHolidayRate,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = jobEvent.EventEndTime
            };

            return rofRevenueForService;
        }

        private decimal CalculateNetRevenueForCompletedService(PetServices petService)
        {
            return petService.Price - petService.EmployeeRate;
        }
    }
}
