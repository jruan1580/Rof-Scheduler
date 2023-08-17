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

        protected async Task<decimal> CalculatePayForCompletedJobEventsByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();

            if (startDate != null && endDate != null)
            {
                for (int i = 0; i < completedEvents.Count; i++)
                {
                    if (completedEvents[i].EventStartTime >= startDate && completedEvents[i].EventEndTime <= endDate)
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

        protected async Task<decimal> CalculateRevenueEarnedByEmployeeByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();

            if (startDate != null && endDate != null)
            {
                for (int i = 0; i < completedEvents.Count; i++)
                {
                    if (completedEvents[i].EventStartTime >= startDate && completedEvents[i].EventEndTime <= endDate)
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

            decimal totalRevenue = 0;

            foreach (var completed in eventsByDate)
            {
                var petService = RofSchedulerMappers.ToCorePetService(await _rofSchedRepo.GetPetServiceById(completed.PetServiceId));

                totalRevenue += petService.Price;
            }

            return totalRevenue;
        }

        protected async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var totalRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, startDate, endDate);
            var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, startDate, endDate);

            var netRevenue = totalRevenue - totalPay;

            return netRevenue;
        }

        protected async Task<bool> CheckIfEventIsHoliday(DateTime jobDate)
        {
            var holiday = await _rofSchedRepo.CheckIfJobDateIsHoliday(jobDate);

            return holiday == null;
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
