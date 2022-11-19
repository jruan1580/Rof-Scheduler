using EventManagementService.API.DTO;
using EventManagementService.API.DtoMapper;
using EventManagementService.Domain.Models;
using NUnit.Framework;
using System;

namespace EventManagementService.Test.Mapper
{
    [TestFixture]
    public class EventDTOMapperTest
    {
        [Test]
        public void ToEventDTOTest()
        {
            var coreEvent = new JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now,
                EventEndTime = DateTime.Now,
                Completed = false,
                Canceled = false,
                Employee = new Employee()
                {
                    Id = 1,
                    FullName = "John Doe",
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

            var dto = EventDTOMapper.ToDTOEvent(coreEvent);

            Assert.IsNotNull(dto);
            Assert.AreEqual(coreEvent.Id, dto.Id);
            Assert.AreEqual(coreEvent.EmployeeId, dto.EmployeeId);
            Assert.AreEqual(coreEvent.PetId, dto.PetId);
            Assert.AreEqual(coreEvent.PetServiceId, dto.PetServiceId);
            Assert.AreEqual(coreEvent.EventStartTime.ToString("yyyy-MM-ddTHH:mm:ss"), dto.EventStartTime);
            Assert.AreEqual(coreEvent.EventEndTime.ToString("yyyy-MM-ddTHH:mm:ss"), dto.EventEndTime);
            Assert.AreEqual(coreEvent.Completed, dto.Completed);
            Assert.AreEqual(coreEvent.Canceled, dto.Canceled);

            Assert.IsNotNull(coreEvent.Employee);
            Assert.AreEqual(dto.EmployeeFullName, coreEvent.Employee.FullName);

            Assert.IsNotNull(coreEvent.Pet);
            Assert.AreEqual(dto.PetName, coreEvent.Pet.Name);

            Assert.IsNotNull(coreEvent.PetService);
            Assert.AreEqual(dto.PetServiceName, coreEvent.PetService.ServiceName);
        }

        [Test]
        public void FromEventDTOTest()
        {
            var dto = new EventDTO()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Completed = false,
                Canceled = false,
                EmployeeFullName = "John Doe",
                PetName = "Dog1",
                PetServiceName = "Walk"
            };

            var coreEvent = EventDTOMapper.FromDTOEvent(dto);

            DateTime dtoCoreDate;

            DateTime.TryParse(dto.EventStartTime, out dtoCoreDate);

            Assert.IsNotNull(coreEvent);
            Assert.AreEqual(dto.Id, coreEvent.Id);
            Assert.AreEqual(dto.EmployeeId, coreEvent.EmployeeId);
            Assert.AreEqual(dto.PetId, coreEvent.PetId);
            Assert.AreEqual(dto.PetServiceId, coreEvent.PetServiceId);
            Assert.AreEqual(dtoCoreDate, coreEvent.EventStartTime);
            Assert.AreEqual(dto.Completed, coreEvent.Completed);
            Assert.AreEqual(dto.Canceled, coreEvent.Canceled);

            Assert.IsNotNull(coreEvent.Employee);
            Assert.AreEqual(coreEvent.Employee.FullName, dto.EmployeeFullName);

            Assert.IsNotNull(coreEvent.Pet);
            Assert.AreEqual(coreEvent.Pet.Name, dto.PetName);

            Assert.IsNotNull(coreEvent.PetService);
            Assert.AreEqual(coreEvent.PetService.ServiceName, dto.PetServiceName);
        }
    }
}
