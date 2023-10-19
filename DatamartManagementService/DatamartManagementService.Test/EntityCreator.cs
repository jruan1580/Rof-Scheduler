using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using System;

namespace DatamartManagementService.Test
{
    public static class EntityCreator
    {
        public static JobExecutionHistory GetDbJobExecutionHistoryRevenue()
        {
            return new JobExecutionHistory()
            {
                Id = 1,
                JobType = "Revenue",
                LastDatePulled = new DateTime(2023, 9, 15)
            };
        }

        public static JobExecutionHistory GetDbJobExecutionHistoryPayroll()
        {
            return new JobExecutionHistory()
            {
                Id = 1,
                JobType = "Payroll",
                LastDatePulled = new DateTime(2023, 9, 15)
            };
        }

        public static JobExecutionHistory GetDbJobExecutionHistoryRevenueSummary()
        {
            return new JobExecutionHistory()
            {
                Id = 1,
                JobType = "Revenue Summary",
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

        public static RofRevenueFromServicesCompletedByDate GetDbDetailedRevenue()
        {
            return new RofRevenueFromServicesCompletedByDate()
            {
                Id = 1,
                PetServiceRate = 20,
                NetRevenuePostEmployeeCut = 5,
                RevenueDate = new DateTime(2023, 10, 09)
            };
        }
    }
}
