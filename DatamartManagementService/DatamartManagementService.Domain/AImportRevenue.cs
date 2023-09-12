using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public abstract class AImportRevenue
    {
        protected readonly IRofSchedRepo _rofSchedRepo;

        public AImportRevenue(IRofSchedRepo rofSchedRepo)
        {
            _rofSchedRepo = rofSchedRepo;
        }

        public abstract Task ImportRevenueData();

        protected async Task<List<JobEvent>> GetCompletedEventsByDate(DateTime startDate, DateTime endDate)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesBetweenDate(startDate, endDate);

            var eventsByDate = new List<JobEvent>();

            for (int i = 0; i < completedEvents.Count; i++)
            {
                eventsByDate.Add(
                    RofSchedulerMappers.ToCoreJobEvent(completedEvents[i]));
            }

            return eventsByDate;
        }

        protected async Task<PetServices> GetPetServiceWithCorrectPayRate(short petServiceId, DateTime jobDate)
        {
            var petService = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(petServiceId));

            await IfDateIsHolidayUpdateRate(petService, jobDate);

            return petService;
        }

        private async Task IfDateIsHolidayUpdateRate(PetServices petService, DateTime jobDate)
        {
            var holiday = await _rofSchedRepo.CheckIfJobDateIsHoliday(jobDate);

            if (holiday != null)
            {
                await UpdateToHolidayPayRate(petService);
            }
        }

        private async Task UpdateToHolidayPayRate(PetServices petService)
        {
            var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(
                    await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id));

            petService.EmployeeRate = holidayRate.HolidayRate;
        }

        //Pay and Revenue Calculation


        protected async Task<decimal> CalculateNetRevenueForCompletedService(PetServices petService)
        {
            var pay = await CalculatePayForCompletedService(petService);

            return petService.Price - pay;
        }

        private async Task<decimal> CalculatePayForCompletedService(PetServices petService)
        {
            decimal grosswageEarnedPerService = 0;

            if (petService.TimeUnit.ToLower() == "hour")
            {
               grosswageEarnedPerService = petService.EmployeeRate * petService.Duration;
            }
            else if (petService.TimeUnit.ToLower() == "min")
            {
                var time = petService.Duration / 60; //gets how many of an hour

                grosswageEarnedPerService = petService.EmployeeRate * time;
            }

            return grosswageEarnedPerService;
        }

        //protected async Task<decimal> CalculateGrossRevenuePerService(long employeeId, DateTime revenueDate)
        //{
        //    var eventsByDate = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

        //    decimal totalRevenue = await CalculateTotalRevenue(eventsByDate);

        //    return totalRevenue;
        //}

        //protected async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime revenueDate)
        //{
        //    var totalRevenue = await CalculateGrossRevenuePerService(employeeId, revenueDate);

        //    var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, revenueDate);

        //    var netRevenue = totalRevenue - totalPay;

        //    return netRevenue;
        //}

        //private async Task<decimal> CalculateTotalGrossPay(List<JobEvent> jobEvents)
        //{
        //    decimal totalGrossPay = 0;

        //    foreach (var job in jobEvents)
        //    {
        //        var petService = RofSchedulerMappers.ToCorePetService(
        //            await _rofSchedRepo.GetPetServiceById(job.PetServiceId));

        //        await IfDateIsHolidayUpdateRate(petService, job.EventStartTime);

        //        decimal grosswageEarnedPerService = await CalculateGrossPay(petService);

        //        totalGrossPay += grosswageEarnedPerService;
        //    }

        //    return totalGrossPay;
        //}

        //private async Task<decimal> CalculateTotalRevenue(List<JobEvent> jobEvents)
        //{
        //    decimal totalRevenue = 0;

        //    foreach (var job in jobEvents)
        //    {
        //        var petService = RofSchedulerMappers.ToCorePetService(await _rofSchedRepo.GetPetServiceById(job.PetServiceId));

        //        totalRevenue += petService.Price;
        //    }

        //    return totalRevenue;
        //}
    }
}
