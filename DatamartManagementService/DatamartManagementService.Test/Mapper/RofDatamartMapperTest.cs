using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DatamartManagementService.Test.Mapper
{
    [TestFixture]
    public class RofDatamartMapperTest
    {
        [Test]
        public void FromCoreRofRevenueFromServicesCompletedByDateTest()
        {
            var core = new List<RofRevenueFromServicesCompletedByDate>()
            {
                new RofRevenueFromServicesCompletedByDate()
                {
                    EmployeeId = 1,
                    EmployeeFirstName = "John",
                    EmployeeLastName = "Doe",
                    PetServiceId = 1,
                    PetServiceName = "Walking",
                    PetServiceRate = 15,
                    IsHolidayRate = false,
                    NetRevenuePostEmployeeCut = 10,
                    RevenueDate = DateTime.Today
                }
            };

            var entity = RofDatamartMappers.FromCoreRofRevenueFromServicesCompletedByDate(core);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity[0].EmployeeId, core[0].EmployeeId);
            Assert.AreEqual(entity[0].EmployeeFirstName, core[0].EmployeeFirstName);
            Assert.AreEqual(entity[0].EmployeeLastName, core[0].EmployeeLastName);
            Assert.AreEqual(entity[0].PetServiceId, core[0].PetServiceId);
            Assert.AreEqual(entity[0].PetServiceName, core[0].PetServiceName);
            Assert.AreEqual(entity[0].PetServiceRate, core[0].PetServiceRate);
            Assert.AreEqual(entity[0].IsHolidayRate, core[0].IsHolidayRate);
            Assert.AreEqual(entity[0].NetRevenuePostEmployeeCut, core[0].NetRevenuePostEmployeeCut);
            Assert.AreEqual(entity[0].RevenueDate, core[0].RevenueDate);
        }

        [Test]
        public void ToCoreJobExecutionHistoryTest()
        {
            var entity = new Infrastructure.Persistence.RofDatamartEntities.JobExecutionHistory()
            {
                Id = 1,
                JobType = "Revenue",
                LastDatePulled = DateTime.Today,
            };

            var coreHistory = RofDatamartMappers.ToCoreJobExecutionHistory(entity);

            Assert.IsNotNull(coreHistory);
            Assert.AreEqual(coreHistory.Id, entity.Id);
            Assert.AreEqual(coreHistory.JobType, entity.JobType);
            Assert.AreEqual(coreHistory.LastDatePulled, entity.LastDatePulled);
        }

        [Test]
        public void FromCoreJobExecutionHistory()
        {
            var core = new JobExecutionHistory()
            {
                Id = 1,
                JobType = "Revenue",
                LastDatePulled = DateTime.Today,
            };

            var entity = RofDatamartMappers.FromCoreJobExecutionHistory(core);

            Assert.IsNotNull(entity);
            Assert.AreEqual(entity.Id, core.Id);
            Assert.AreEqual(entity.JobType, core.JobType);
            Assert.AreEqual(entity.LastDatePulled, core.LastDatePulled);
        }
    }
}
