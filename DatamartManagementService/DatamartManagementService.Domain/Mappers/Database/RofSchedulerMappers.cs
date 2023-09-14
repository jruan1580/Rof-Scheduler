using CoreJobEvent = DatamartManagementService.Domain.Models.RofSchedulerModels.JobEvent;
using DbJobEvent = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.JobEvent;
using CoreEmployee = DatamartManagementService.Domain.Models.RofSchedulerModels.Employee;
using DbEmployee = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.Employee;
using CorePetService = DatamartManagementService.Domain.Models.RofSchedulerModels.PetServices;
using DbPetService = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.PetServices;
using CoreHoliday = DatamartManagementService.Domain.Models.RofSchedulerModels.Holiday;
using DbHoliday = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.Holidays;
using CoreHolidayRate = DatamartManagementService.Domain.Models.RofSchedulerModels.HolidayRates;
using DbHolidayRate = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.HolidayRates;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DatamartManagementService.Domain.Mappers.Database
{
    public static class RofSchedulerMappers
    {
        public static List<CoreJobEvent> ToCoreJobEvents(List<DbJobEvent> dbJobEvents)
        {
            var coreEvents = new List<CoreJobEvent>();

            foreach(var dbJob in dbJobEvents)
            {
                coreEvents.Add(new CoreJobEvent()
                {
                    Id = dbJob.Id,
                    EmployeeId = dbJob.EmployeeId,
                    PetServiceId = dbJob.PetServiceId,
                    EventStartTime = dbJob.EventStartTime,
                    EventEndTime = dbJob.EventEndTime,
                    Completed = dbJob.Completed
                });
            }

            return coreEvents;
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

        public static CoreHoliday ToCoreHoliday(DbHoliday dbHoliday)
        {
            var coreHoliday = new CoreHoliday();

            coreHoliday.Id = dbHoliday.Id;
            coreHoliday.HolidayName = dbHoliday.HolidayName;
            coreHoliday.HolidayMonth = dbHoliday.HolidayMonth;
            coreHoliday.HolidayDay = dbHoliday.HolidayDay;

            return coreHoliday;
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
