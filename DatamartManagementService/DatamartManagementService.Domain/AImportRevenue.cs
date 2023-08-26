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

            decimal totalGrossPay = 0;

            foreach (var completed in eventsByDate)
            {
                var petService = RofSchedulerMappers.ToCorePetService(await _rofSchedRepo.GetPetServiceById(completed.PetServiceId));
                var jobEvent = RofSchedulerMappers.ToCoreJobEvent(await _rofSchedRepo.GetJobEventById(completed.Id));
                var isHoliday = await CheckIfEventIsHoliday(jobEvent.EventStartTime);

                if (isHoliday)
                {
                    var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id));

                    petService.EmployeeRate = holidayRate.HolidayRate;
                }

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

                totalGrossPay += grosswageEarnedPerService;
            }

            return totalGrossPay;
        }

        protected async Task<decimal> CalculateRevenueEarnedByEmployeeByDate(long employeeId, DateTime revenueDate)
        {
            var eventsByDate = await GetEmployeeCompletedEventsByDate(employeeId, revenueDate);

            decimal totalRevenue = 0;

            foreach (var completed in eventsByDate)
            {
                var petService = RofSchedulerMappers.ToCorePetService(await _rofSchedRepo.GetPetServiceById(completed.PetServiceId));

                totalRevenue += petService.Price;
            }

            return totalRevenue;
        }

        protected async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime revenueDate)
        {
            var totalRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, revenueDate);
            var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, revenueDate);

            var netRevenue = totalRevenue - totalPay;

            return netRevenue;
        }

        protected async Task<bool> CheckIfEventIsHoliday(DateTime jobDate)
        {
            var holiday = await _rofSchedRepo.CheckIfJobDateIsHoliday(jobDate);

            return holiday == null;
        }

        private async Task<List<JobEvent>> GetEmployeeCompletedEventsByDate(long employeeId, DateTime date)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();

            if (date != null)
            {
                for (int i = 0; i < completedEvents.Count; i++)
                {
                    if (completedEvents[i].EventEndTime.Date == date.Date)
                    {
                        eventsByDate.Add(RofSchedulerMappers.ToCoreJobEvent(completedEvents[i]));
                    }
                }
            }
            else
            {
                foreach (var jobEvent in completedEvents)
                {
                    eventsByDate.Add(RofSchedulerMappers.ToCoreJobEvent(jobEvent));
                }
            }

            return eventsByDate;
        }

        //private async Task<decimal> CalculatateTotalRevenueByDate(List<RofRevenueFromServicesCompletedByDate> rofRevenueFromServicesCompletedByDates, 
        //    DateTime startDate, DateTime endDate)
        //{
        //    decimal totalRevenue = 0;

        //    foreach(var rofRevenue in rofRevenueFromServicesCompletedByDates)
        //    {
        //        var revenuePerEmployee = await CalculateRevenueEarnedByEmployeeByDate(rofRevenue.EmployeeId, startDate, endDate);

        //        totalRevenue += revenuePerEmployee;
        //    }

        //    return totalRevenue;
        //}
    }
}
