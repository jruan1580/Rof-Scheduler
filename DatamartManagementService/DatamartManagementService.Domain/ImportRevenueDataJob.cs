using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRevenueDataJob : BackgroundService
    {
        private readonly int _hoursInBetweenRun = 12;
        private readonly IRofSchedRepo _rofSchedRepo;

        public ImportRevenueDataJob(IRofSchedRepo rofSchedRepo)
        {
            _rofSchedRepo = rofSchedRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //keep running unless told to stop AKA told to cancel
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: import data
                Console.WriteLine("hello from the job");

                await Task.Delay(TimeSpan.FromHours(_hoursInBetweenRun), stoppingToken);
            }       
        }

        public async Task<EmployeePayroll> PopulateEmployeePayroll(long employeeId, DateTime startDate, DateTime endDate)
        {
            var employeeInfo = await _rofSchedRepo.GetEmployeeById(employeeId);
            var employeeTotalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, startDate, endDate);

            var newEmployeePayroll = new EmployeePayroll()
            {
                EmployeeId = employeeInfo.Id,
                FirstName = employeeInfo.FirstName,
                LastName = employeeInfo.LastName,
                EmployeeTotalPay = employeeTotalPay,
                PayPeriodStartDate = startDate,
                PayPeriodEndDate = endDate
            };

            return newEmployeePayroll;
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

        private async Task<decimal> CalculatePayForCompletedJobEventsByDate(long employeeId, DateTime startDate, DateTime endDate)
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
                else if(petService.TimeUnit.ToLower() == "min")
                {
                    var time = petService.Duration / 60; //gets how many of an hour

                    grosswageEarnedPerService = petService.EmployeeRate * time;
                }

                totalGrossPay += grosswageEarnedPerService;
            }

            return totalGrossPay;
        }

        private async Task<decimal> CalculateRevenueEarnedByEmployeeByDate(long employeeId, DateTime startDate, DateTime endDate)
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

        private async Task<decimal> CalculateNetRevenueEarnedByDate(long employeeId, DateTime startDate, DateTime endDate)
        {
            var totalRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeeId, startDate, endDate);
            var totalPay = await CalculatePayForCompletedJobEventsByDate(employeeId, startDate, endDate);

            var netRevenue = totalRevenue - totalPay;

            return netRevenue;
        }

        private async Task<bool> CheckIfEventIsHoliday(DateTime jobDate)
        {
            var holiday = await _rofSchedRepo.CheckIfJobDateIsHoliday(jobDate);

            return holiday == null;
        }
    }
}
