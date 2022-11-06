using EventManagementService.Domain.Mappers;
using EventManagementService.Infrastructure.Persistence.Entities;
using NUnit.Framework;
using System;

namespace EventManagementService.Test.Mapper
{
    [TestFixture]
    public class EventMapperTest
    {
        [Test]
        public void ToCoreEventTest()
        {
            var entity = new JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventDate = DateTime.Today,
                Completed = false,
                Canceled = false,
                Employee = new Employee()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "Admin"
                },
                Pet = new Pet()
                {
                    Id = 1,
                    Name = "Dog1"
                },
                PetService = new PetService()
                {
                    Id = 1,
                    ServiceName = "Walk"
                }
            };

            var coreEvent = EventMapper.ToCoreEvent(entity);

            Assert.IsNotNull(coreEvent);
            Assert.AreEqual(entity.Id, coreEvent.Id);
            Assert.AreEqual(entity.EmployeeId, coreEvent.EmployeeId);
            Assert.AreEqual(entity.PetId, coreEvent.PetId);
            Assert.AreEqual(entity.PetServiceId, coreEvent.PetServiceId);
            Assert.AreEqual(entity.EventDate, coreEvent.EventDate);
            Assert.AreEqual(entity.Completed, coreEvent.Completed);
            Assert.AreEqual(entity.Canceled, coreEvent.Canceled);

            Assert.IsNotNull(coreEvent.Employee);
            Assert.AreEqual(entity.Employee.Id, coreEvent.Employee.Id);
            Assert.AreEqual(entity.Employee.FirstName, coreEvent.Employee.FirstName);
            Assert.AreEqual(entity.Employee.LastName, coreEvent.Employee.LastName);
            Assert.AreEqual(entity.Employee.Role, coreEvent.Employee.Role);

            Assert.IsNotNull(coreEvent.Pet);
            Assert.AreEqual(entity.Pet.Id, coreEvent.Pet.Id);
            Assert.AreEqual(entity.Pet.Name, coreEvent.Pet.Name);

            Assert.IsNotNull(coreEvent.PetService);
            Assert.AreEqual(entity.PetService.Id, coreEvent.PetService.Id);
            Assert.AreEqual(entity.PetService.ServiceName, coreEvent.PetService.ServiceName);
        }

        [Test]
        public void FromCoreEventTest()
        {
            var coreEvent = new Domain.Models.JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventDate = DateTime.Today,
                Completed = false,
                Canceled = false,
                Employee = new Domain.Models.Employee()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "Admin"
                },
                Pet = new Domain.Models.Pet()
                {
                    Id = 1,
                    Name = "Dog1"
                },
                PetService = new Domain.Models.PetService()
                {
                    Id = 1,
                    ServiceName = "Walk"
                }
            };

            var entity = EventMapper.FromCoreEvent(coreEvent);

            Assert.IsNotNull(entity);
            Assert.AreEqual(coreEvent.Id, entity.Id);
            Assert.AreEqual(coreEvent.EmployeeId, entity.EmployeeId);
            Assert.AreEqual(entity.PetId, coreEvent.PetId);
            Assert.AreEqual(entity.PetServiceId, coreEvent.PetServiceId);
            Assert.AreEqual(entity.EventDate, coreEvent.EventDate);
            Assert.AreEqual(entity.Completed, coreEvent.Completed);
            Assert.AreEqual(entity.Canceled, coreEvent.Canceled);
        }
    }
}
