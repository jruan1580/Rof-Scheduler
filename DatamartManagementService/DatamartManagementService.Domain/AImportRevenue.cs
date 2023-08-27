using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
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

        protected async Task<decimal> CalculatePayForCompletedJobEventsByDate(long employeeId, DateTime revenueDate)
        {
            var eventsByDate = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

            decimal totalGrossPay = await CalculateTotalGrossPay(eventsByDate);

            return totalGrossPay;
        }

        protected async Task<decimal> CalculateRevenueEarnedByEmployeeByDate(long employeeId, DateTime revenueDate)
        {
            var eventsByDate = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

            decimal totalRevenue = await CalculateTotalRevenue(eventsByDate);

            return totalRevenue;
        }

        protected async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime revenueDate)
        {
            var totalRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, revenueDate);

            var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, revenueDate);

            var netRevenue = totalRevenue - totalPay;

            return netRevenue;
        }

        protected async Task<PetServices> GetPetServicePayRate(JobEvent jobEvent)
        {
            var petService = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(jobEvent.PetServiceId));

            await IfEventIsHolidayUpdateRate(petService, jobEvent.EventStartTime);

            return petService;
        }

        private async Task IfEventIsHolidayUpdateRate(PetServices petService, DateTime jobDate)
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

        private async Task<decimal> CalculateTotalGrossPay(List<JobEvent> jobEvents)
        {
            decimal totalGrossPay = 0;

            foreach (var job in jobEvents)
            {
                var petService = RofSchedulerMappers.ToCorePetService(
                    await _rofSchedRepo.GetPetServiceById(job.PetServiceId));

                await IfEventIsHolidayUpdateRate(petService, job.EventStartTime);

                decimal grosswageEarnedPerService = await CalculateGrossWages(petService);

                totalGrossPay += grosswageEarnedPerService;
            }

            return totalGrossPay;
        }

        private async Task<decimal> CalculateGrossWages(PetServices petService)
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

        private async Task<decimal> CalculateTotalRevenue(List<JobEvent> jobEvents)
        {
            decimal totalRevenue = 0;

            foreach (var job in jobEvents)
            {
                var petService = RofSchedulerMappers.ToCorePetService(await _rofSchedRepo.GetPetServiceById(job.PetServiceId));

                totalRevenue += petService.Price;
            }

            return totalRevenue;
        }

        private async Task<List<JobEvent>> GetEmployeeCompletedEventsByDate(long employeeId, DateTime date)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();

            for (int i = 0; i < completedEvents.Count; i++)
            {
                if (completedEvents[i].EventEndTime.Date == date.Date)
                {
                    eventsByDate.Add(RofSchedulerMappers.ToCoreJobEvent(completedEvents[i]));
                }
            }

            return eventsByDate;
        }        
    }
}
