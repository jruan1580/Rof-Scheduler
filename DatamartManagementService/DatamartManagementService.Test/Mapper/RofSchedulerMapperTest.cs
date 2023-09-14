using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Test.Mapper
{
    [TestFixture]
    public class RofSchedulerMapperTest
    {
        [Test]
        public void ToCoreJobEvents()
        {
            var entities = new List<JobEvent>()
            {
                new JobEvent()
                {
                    Id = 1,
                    EmployeeId = 1,
                    PetServiceId = 1,
                    EventStartTime = DateTime.Today,
                    EventEndTime = DateTime.Today.AddDays(1),
                    Completed = true
                }
            };

            var core = RofSchedulerMappers.ToCoreJobEvents(entities);

            Assert.IsNotNull(entities[0]);
            Assert.AreEqual(entities[0].Id, core[0].Id);
            Assert.AreEqual(entities[0].EmployeeId, core[0].EmployeeId);
            Assert.AreEqual(entities[0].PetServiceId, core[0].PetServiceId);
            Assert.AreEqual(entities[0].EventStartTime, core[0].EventStartTime);
            Assert.AreEqual(entities[0].EventEndTime, core[0].EventEndTime);
        }

        [Test]
        public void ToCoreEmployee()
        {
            var entity = new Employee()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            var core = RofSchedulerMappers.ToCoreEmployee(entity);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, core.Id);
            Assert.AreEqual(entity.FirstName, core.FirstName);
            Assert.AreEqual(entity.LastName, core.LastName);
        }

        [Test]
        public void ToCorePetService()
        {
            var entity = new PetServices()
            {
                Id = 1,
                ServiceName = "Walking",
                Price = 25,
                EmployeeRate = 15,
                Duration = 30,
                TimeUnit = "Min"
            };

            var core = RofSchedulerMappers.ToCorePetService(entity);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, core.Id);
            Assert.AreEqual(entity.ServiceName, core.ServiceName);
            Assert.AreEqual(entity.Price, core.Price);
            Assert.AreEqual(entity.EmployeeRate, core.EmployeeRate);
            Assert.AreEqual(entity.Duration, core.Duration);
            Assert.AreEqual(entity.TimeUnit, core.TimeUnit);
        }

        public void ToCoreHoliday()
        {
            var entity = new Holidays()
            {
                Id = 1,
                HolidayName = "Christmas",
                HolidayMonth = 12,
                HolidayDay = 25
            };

            var core = RofSchedulerMappers.ToCoreHoliday(entity);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, core.Id);
            Assert.AreEqual(entity.HolidayName, core.HolidayName);
            Assert.AreEqual(entity.HolidayMonth, core.HolidayMonth);
            Assert.AreEqual(entity.HolidayDay, core.HolidayDay);
        }

        public void ToCoreHolidayRate()
        {
            var entity = new HolidayRates()
            {
                Id = 1,
                HolidayId = 1,
                PetServiceId = 1,
                HolidayRate = 23
            };

            var core = RofSchedulerMappers.ToCoreHolidayRate(entity);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, core.Id);
            Assert.AreEqual(entity.HolidayId, core.HolidayId);
            Assert.AreEqual(entity.PetServiceId, core.PetServiceId);
            Assert.AreEqual(entity.HolidayRate, core.HolidayRate);
        }
    }
}
