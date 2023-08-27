using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.RofDatamartRepos;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobEvent = DatamartManagementService.Domain.Models.RofSchedulerModels.JobEvent;

namespace DatamartManagementService.Domain
{
    public interface IImportRofRevenueFromServicesCompletedByDate
    {
        Task ImportRevenueData();
    }

    public class ImportRofRevenueFromServicesCompletedByDate : AImportRevenue, IImportRofRevenueFromServicesCompletedByDate
    {
        private readonly DateTime? _lastRevenueDateProcessed;
        private readonly IRevenueFromServicesUpsertRepository _singleRevenueUpsertRepo;

        public ImportRofRevenueFromServicesCompletedByDate(IRofSchedRepo rofSchedRepo,
            IRevenueFromServicesUpsertRepository singleRevenueUpsertRepo)
            : base(rofSchedRepo)
        {
            _singleRevenueUpsertRepo = singleRevenueUpsertRepo;
        }

        public override async Task ImportRevenueData()
        {
            //call from rof scheduler db to give you data since lastRevenueDateProcessed
            var employees = RofSchedulerMappers.ToCoreEmployees(
                await _rofSchedRepo.GetEmployees());
            var petServices = RofSchedulerMappers.ToCorePetServices(
                await _rofSchedRepo.GetPetServices());

            var employeeIds = new List<long>();
            var petServiceIds = new List<short>();

            foreach(var employee in employees)
            {
                employeeIds.Add(employee.Id);
            }

            foreach(var petService in petServices)
            {
                petServiceIds.Add(petService.Id);
            }

            //and if lastRevenueDateProcessed is null, import data from yesterday

            var listOfSingleRevenues = RofDatamartMappers.FromCoreRofRevenueFromServicesCompletedByDate(
                await PopulateListOfRofRevenueOfCompletedServiceByDate(employeeIds, petServiceIds, _lastRevenueDateProcessed));

            await _singleRevenueUpsertRepo.AddRevenueFromServices(listOfSingleRevenues);
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(
            List<long> employeeIds,
            List<short> petServiceIds, 
            DateTime? revenueDate)
        {
            var revDate = new DateTime();
            if (revenueDate == null)
            {
                revDate = DateTime.Today.AddDays(-1);
            }

            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var employeeId in employeeIds)
            {
                foreach (var petServiceId in petServiceIds)
                {
                    var revenue = await PopulateRofRevenueForServicesCompletedByDate(employeeId, petServiceId, revDate);

                    revenueForServiceCompleted.Add(revenue);
                }
            }
            
            revenueDate = revDate;

            return revenueForServiceCompleted;
        }

        private async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(long employeeId, short petServiceId, DateTime revenueDate)
        {
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(employeeId));

            var petServiceInfo = await GetPetServiceWitPayRate(petServiceId, revenueDate);

            var completedEvents = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

            var completedPetServiceEventsForTheDay = await PullCompletedJobEventsOfAPetServiceForTheDay(completedEvents, petServiceId);

            var isHolidayRate = await CheckIfHolidayRate(revenueDate);

            var netRevenue = await CalculateTotalNetRevenueFromPetServiceComplete(completedPetServiceEventsForTheDay, revenueDate);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeeInfo.Id,
                EmployeeFirstName = employeeInfo.FirstName,
                EmployeeLastName = employeeInfo.LastName,
                EmployeePay = petServiceInfo.EmployeeRate,
                PetServiceId = petServiceInfo.Id,
                PetServiceName = petServiceInfo.ServiceName,
                PetServiceRate = petServiceInfo.EmployeeRate,
                IsHolidayRate = isHolidayRate,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = revenueDate
            };

            return rofRevenueForService;
        }

        private async Task<List<JobEvent>> PullCompletedJobEventsOfAPetServiceForTheDay(List<JobEvent> completedEvents, short petServiceId)
        {
            var completedPetServiceEventsForTheDay = new List<JobEvent>();

            foreach (var completed in completedEvents)
            {
                if (completed.PetServiceId == petServiceId)
                {
                    completedPetServiceEventsForTheDay.Add(completed);
                }
            }

            return completedPetServiceEventsForTheDay;
        }

        private async Task<decimal> CalculateTotalNetRevenueFromPetServiceComplete(List<JobEvent> completedEvents, DateTime revenueDate)
        {
            decimal netRevenue = 0;

            foreach (var completed in completedEvents)
            {
                netRevenue += await CalculateNetRevenueEarnedByDate(completed.EmployeeId, revenueDate);
            }

            return netRevenue;
        }

        private async Task<bool> CheckIfHolidayRate(DateTime revenueDate)
        {
            var isHoliday = RofSchedulerMappers.ToCoreHoliday(
                await _rofSchedRepo.CheckIfJobDateIsHoliday(revenueDate));

            var isHolidayRate = (isHoliday != null);

            return isHolidayRate;
        }
    }
}
