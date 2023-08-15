using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.RofDatamartRepos;
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
        private readonly IRevenueByDateUpsertRepository _revenueByDateUpsertRepo;
        private readonly IRevenueFromServicesUpsertRepository revenueFromServicesUpsertRepo;

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

        public async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(List<EmployeePayrollDetail> employees)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var employee in employees)
            {
                var revenue = await PopulateRofRevenueForServicesCompletedByDate(employee);

                revenueForServiceCompleted.Add(revenue);
            }

            return revenueForServiceCompleted;
        }

        public async Task<List<RofRevenueByDate>> PopulateListOfRofRevenueByDate(List<EmployeePayrollDetail> employees)
        {
            var revenueByDate = new List<RofRevenueByDate>();

            foreach (var employee in employees)
            {
                var revenue = await PopulateRofRevenueByDate(employee);

                revenueByDate.Add(revenue);
            }

            return revenueByDate;
        }
        
        private async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(EmployeePayrollDetail employeePayrollDetail)
        {
            var petService = await _rofSchedRepo.GetPetServiceById(employeePayrollDetail.PetServiceId);

            var netRevenue = await CalculateNetRevenueEarnedByDate(employeePayrollDetail.EmployeeId,
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var rofRevenueForService = new RofRevenueFromServicesCompletedByDate()
            {
                EmployeeId = employeePayrollDetail.EmployeeId,
                EmployeeFirstName = employeePayrollDetail.FirstName,
                EmployeeLastName = employeePayrollDetail.LastName,
                EmployeePay = employeePayrollDetail.EmployeePayForService,
                PetServiceId = employeePayrollDetail.PetServiceId,
                PetServiceName = employeePayrollDetail.PetServiceName,
                PetServiceRate = petService.EmployeeRate,
                IsHolidayRate = employeePayrollDetail.IsHolidayPay,
                NetRevenuePostEmployeeCut = netRevenue,
                RevenueDate = employeePayrollDetail.ServiceEndDateTime
            };

            return rofRevenueForService;
        }

        private async Task<RofRevenueByDate> PopulateRofRevenueByDate(EmployeePayrollDetail employeePayrollDetail)
        {
            var grossRevenue = await CalculateRevenueEarnedByEmployeeByDate(employeePayrollDetail.EmployeeId, 
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var netRevenue = await CalculateNetRevenueEarnedByDate(employeePayrollDetail.EmployeeId,
                employeePayrollDetail.ServiceStartDateTime, employeePayrollDetail.ServiceEndDateTime);

            var revenueByDate = new RofRevenueByDate()
            {
                RevenueDate = employeePayrollDetail.ServiceEndDateTime,
                RevenueMonth = Convert.ToInt16(employeePayrollDetail.ServiceEndDateTime.Month),
                RevenueYear = Convert.ToInt16(employeePayrollDetail.ServiceEndDateTime.Year),
                GrossRevenue = grossRevenue,
                NetRevenuePostEmployeePay = netRevenue
            };

            return revenueByDate;
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

        //Payroll / Payroll Detail Methods



        public async Task<List<EmployeePayroll>> PopulateListOfEmployeePayroll(List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            var employeePayrolls = new List<EmployeePayroll>();

            foreach (var employee in employeeIds)
            {
                var payroll = await PopulateEmployeePayroll(employee, startDate, endDate);
                employeePayrolls.Add(payroll);
            }

            return employeePayrolls;
        }

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

        private async Task<EmployeePayroll> PopulateEmployeePayroll(long employeeId, DateTime startDate, DateTime endDate)
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

        private async Task<EmployeePayrollDetail> PopulateEmployeePayrollDetail(long employeeId, short petServiceId, int jobEventId)
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
