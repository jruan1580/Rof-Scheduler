using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public abstract class AImportRevenuePayroll
    {
        private readonly IRofSchedRepo _rofSchedRepo;

        public async Task<decimal> CalculatePayForCompletedJobEventsByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();
            if (startDate != null && endDate != null)
            {
                for (int i = 0; i < completedEvents.Count; i++)
                {
                    if (completedEvents[i].EventStartTime >= startDate && completedEvents[i].EventEndTime <= endDate)
                    {
                        eventsByDate.Add(completedEvents[i]);
                    }
                }
            }
            else
            {
                eventsByDate.AddRange(completedEvents);
            }

            decimal totalGrossPay = 0;

            foreach (var completed in eventsByDate)
            {
                var petService = await _rofSchedRepo.GetPetServiceById(completed.PetServiceId);
                var jobEvent = await _rofSchedRepo.GetJobEventById(completed.Id);
                var isHoliday = await CheckIfEventIsHoliday(jobEvent.EventStartTime);

                if (isHoliday)
                {
                    var holidayRate = await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id);

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

        public async Task<decimal> CalculateRevenueEarnedByEmployeeByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var completedEvents = await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId);

            var eventsByDate = new List<JobEvent>();

            if (startDate != null && endDate != null)
            {
                for (int i = 0; i < completedEvents.Count; i++)
                {
                    if (completedEvents[i].EventStartTime >= startDate && completedEvents[i].EventEndTime <= endDate)
                    {
                        eventsByDate.Add(completedEvents[i]);
                    }
                }
            }
            else
            {
                eventsByDate.AddRange(completedEvents);
            }

            decimal totalRevenue = 0;

            foreach (var completed in eventsByDate)
            {
                var petService = await _rofSchedRepo.GetPetServiceById(completed.PetServiceId);
                var jobEvent = await _rofSchedRepo.GetJobEventById(completed.Id);

                totalRevenue += petService.Price;
            }

            return totalRevenue;
        }

        public async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var totalRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, startDate, endDate);
            var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, startDate, endDate);

            var netRevenue = totalRevenue - totalPay;

            return netRevenue;
        }

        public async Task<bool> CheckIfEventIsHoliday(DateTime jobDate)
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
