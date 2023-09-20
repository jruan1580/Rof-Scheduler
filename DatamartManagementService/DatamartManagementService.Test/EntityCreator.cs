using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Test
{
    public static class EntityCreator
    {
        public static JobExecutionHistory GetDbJobExecutionHistory()
        {
            return new JobExecutionHistory()
            {
                Id = 1,
                JobType = "Revenue",
                LastDatePulled = new DateTime(2023, 9, 15)
            };
        }

        public static JobEvent GetDbJobEvent()
        {
            return new JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetServiceId = 1,
                EventStartTime = new DateTime(2023, 9, 16, 7, 30, 0),
                EventEndTime = new DateTime(2023, 9, 16, 8, 30, 0),
                Completed = true,
                LastModifiedDateTime = new DateTime(2023, 9, 17, 8, 0, 0)
            };
        }

        public static Employee GetDbEmployee()
        {
            return new Employee()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };
        }

        public static PetServices GetDbPetService()
        {
            return new PetServices()
            {
                Id = 1,
                ServiceName = "Walking",
                Price = 25,
                EmployeeRate = 15,
                Duration = 1,
                TimeUnit = "Hour"
            };
        }

        public static Holidays GetDbHoliday()
        {
            return new Holidays()
            {
                Id = 1,
                HolidayName = "Christmas",
                HolidayMonth = 12,
                HolidayDay = 25,
            };
        }

        public static HolidayRates GetDbHolidayRates()
        {
            return new HolidayRates()
            {
                Id = 1,
                HolidayId = 1,
                PetServiceId = 1,
                HolidayRate = 23
            };
        }
    }
}
