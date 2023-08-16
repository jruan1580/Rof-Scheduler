﻿using CoreJobEvent = DatamartManagementService.Domain.Models.RofSchedulerModels.JobEvent;
using DbJobEvent = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.JobEvent;
using CoreEmployee = DatamartManagementService.Domain.Models.RofSchedulerModels.Employee;
using DbEmployee = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.Employee;
using CorePetService = DatamartManagementService.Domain.Models.RofSchedulerModels.PetServices;
using DbPetService = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.PetServices;
using CoreHolidayRate = DatamartManagementService.Domain.Models.RofSchedulerModels.HolidayRates;
using DbHolidayRate = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.HolidayRates;

namespace DatamartManagementService.Domain.Mappers.Database
{
    public static class RofSchedulerMappers
    {
        public static CoreJobEvent ToCoreJobEvent(DbJobEvent dbJobEvent)
        {
            var coreEvent = new CoreJobEvent();

            coreEvent.Id = dbJobEvent.Id;
            coreEvent.EmployeeId = dbJobEvent.EmployeeId;
            coreEvent.PetServiceId = dbJobEvent.PetServiceId;
            coreEvent.EventStartTime = dbJobEvent.EventStartTime;
            coreEvent.EventEndTime = dbJobEvent.EventEndTime;
            coreEvent.Completed = dbJobEvent.Completed;

            return coreEvent;
        }

        public static CoreEmployee ToCoreEmployee(DbEmployee dbEmployee)
        {
            var coreEmployee = new CoreEmployee();

            coreEmployee.Id = dbEmployee.Id;
            coreEmployee.FirstName = dbEmployee.FirstName;
            coreEmployee.LastName = dbEmployee.LastName;

            return coreEmployee;
        }

        public static CorePetService ToCorePetService(DbPetService dbPetService)
        {
            var corePetService = new CorePetService();

            corePetService.Id = dbPetService.Id;
            corePetService.ServiceName = dbPetService.ServiceName;
            corePetService.Price = dbPetService.Price;
            corePetService.EmployeeRate = dbPetService.EmployeeRate;
            corePetService.Duration = dbPetService.Duration;
            corePetService.TimeUnit = dbPetService.TimeUnit;

            return corePetService;
        }

        public static CoreHolidayRate ToCoreHolidayRate(DbHolidayRate dbHolidayRate)
        {
            var coreHolidayRate = new CoreHolidayRate();

            coreHolidayRate.Id = dbHolidayRate.Id;
            coreHolidayRate.HolidayId = dbHolidayRate.HolidayId;
            coreHolidayRate.PetServiceId = dbHolidayRate.PetServiceId;
            coreHolidayRate.HolidayRate = dbHolidayRate.HolidayRate;

            return coreHolidayRate;
        }
    }
}
