﻿using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public interface IImportRofRevenueFromServicesCompletedByDate
    {
        Task ImportRevenueData();
    }

    public class ImportRofRevenueFromServicesCompletedByDate : AImportRevenue, IImportRofRevenueFromServicesCompletedByDate
    {
        private readonly DateTime? _lastRevenueDateProcessed;

        public ImportRofRevenueFromServicesCompletedByDate(IRofSchedRepo rofSchedRepo)
            : base(rofSchedRepo) { }

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
            //and if lastRevenueDateProcessed is null, get all data up to yesterday
            
            await PopulateListOfRofRevenueOfCompletedServiceByDate(employeeIds, petServiceIds, _lastRevenueDateProcessed.Value);
        }

        private async Task<List<RofRevenueFromServicesCompletedByDate>> PopulateListOfRofRevenueOfCompletedServiceByDate(
            List<long> employeeIds,
            List<short> petServiceIds, 
            DateTime revenueDate)
        {
            var revenueForServiceCompleted = new List<RofRevenueFromServicesCompletedByDate>();

            foreach(var employeeId in employeeIds)
            {
                foreach (var petServiceId in petServiceIds)
                {
                    var revenue = await PopulateRofRevenueForServicesCompletedByDate(employeeId, petServiceId, revenueDate);

                    revenueForServiceCompleted.Add(revenue);
                }
            }            

            return revenueForServiceCompleted;
        }

        // Populate revenue for a single employee for a single petservice completed for a single day
        private async Task<RofRevenueFromServicesCompletedByDate> PopulateRofRevenueForServicesCompletedByDate(long employeeId, short petServiceId, DateTime singleDate)
        {
            var employeeInfo = RofSchedulerMappers.ToCoreEmployee(
                await _rofSchedRepo.GetEmployeeById(employeeId));
            var petServiceInfo = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(petServiceId));
            var isHoliday = RofSchedulerMappers.ToCoreHoliday(
                await _rofSchedRepo.CheckIfJobDateIsHoliday(singleDate));
            var completedEvents = RofSchedulerMappers.ToCoreJobEvents(
                await _rofSchedRepo.GetCompletedServicesDoneByEmployee(employeeId));

            var completedPetServiceEventsForTheDay = new List<JobEvent>();

            foreach(var completed in completedEvents)
            {
                if(completed.PetServiceId == petServiceInfo.Id && completed.EventEndTime.Date == singleDate.Date)
                {
                    completedPetServiceEventsForTheDay.Add(completed);
                }
            }

            var isHolidayRate = false;

            if(isHoliday != null)
            {
                var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(await _rofSchedRepo.GetHolidayRateByPetServiceId(petServiceId));
                petServiceInfo.EmployeeRate = holidayRate.HolidayRate;
                isHolidayRate = true;
            }

            decimal netRevenue = 0;

            foreach (var completed in completedPetServiceEventsForTheDay)
            {
                netRevenue += await CalculateNetRevenueEarnedByDate(employeeInfo.Id, singleDate.Date, singleDate.Date);
            }

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
                RevenueDate = singleDate
            };

            return rofRevenueForService;
        }
    }
}
