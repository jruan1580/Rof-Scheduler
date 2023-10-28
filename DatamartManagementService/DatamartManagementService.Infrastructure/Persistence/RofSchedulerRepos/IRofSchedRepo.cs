﻿using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos
{
    public interface IRofSchedRepo
    {
        Task<Holidays> CheckIfJobDateIsHoliday(DateTime jobDate);
        Task<List<JobEvent>> GetCompletedServicesBetweenDates(DateTime startDate, DateTime endDate);
        Task<List<JobEvent>> GetCompletedServicesUpUntilDate(DateTime date);
        Task<Employee> GetEmployeeById(long id);
        Task<HolidayRates> GetHolidayRateByPetServiceId(short petServiceId);
        Task<List<PetServices>> GetAllPetServices();
        Task<PetServices> GetPetServiceById(short id);
    }
}