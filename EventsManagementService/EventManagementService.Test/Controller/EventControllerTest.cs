using EventManagementService.API.Controllers;
using EventManagementService.API.DTO;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementService.Test.Controller
{
    [TestFixture]
    public class EventControllerTest
    {
        private Mock<IEventService> _eventService;

        [SetUp]
        public void Setup()
        {
            _eventService = new Mock<IEventService>();
        }

        [Test]
        public async Task AddEvent_Success()
        {
            var newEvent = new EventDTO()
            {
                EmployeeId = 1,
                EmployeeFullName = "John Doe",
                PetId = 1,
                PetName = "Dog1",
                PetServiceId = 1,
                PetServiceName = "Walk",
                EventDate = DateTime.Now,
                Completed = false,
                Canceled = false
            };

            _eventService.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var controller = new EventController(_eventService.Object);

            var response = await controller.AddEvent(newEvent);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());
        }

        [Test]
        public async Task AddEvent_BadRequest()
        {
            var newEvent = new EventDTO()
            {
                EmployeeId = 1,
                EmployeeFullName = "John Doe",
                PetId = 1,
                PetName = "Dog1",
                PetServiceId = 1,
                PetServiceName = "Walk",
                EventDate = DateTime.Now,
                Completed = false,
                Canceled = false
            };

            _eventService.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new ArgumentException("test"));

            var controller = new EventController(_eventService.Object);

            var response = await controller.AddEvent(newEvent);

            Assert.IsNotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var badRequestObj = (BadRequestObjectResult)response;
            Assert.AreEqual(typeof(string), badRequestObj.Value.GetType());
            Assert.AreEqual("test", badRequestObj.Value.ToString());
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_Success()
        {
            var events = new List<JobEvent>()
            {
                new JobEvent()
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
                        FullName = "John Doe"
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
                }
            };

            _eventService.Setup(e => e.GetAllJobEventsByMonthAndYear(It.IsAny<DateTime>()))
                .ReturnsAsync(events);

            var controller = new EventController(_eventService.Object);

            var response = await controller.GetAllJobEventsByMonthAndYear(DateTime.Today);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;
            Assert.IsNotNull(okObj);
            Assert.AreEqual(okObj.StatusCode, 200);

            Assert.AreEqual(typeof(List<EventDTO>), okObj.Value.GetType());
            
            var eventDTO = (List<EventDTO>)okObj.Value;
            Assert.AreEqual(1, eventDTO.Count);
            Assert.AreEqual(events[0].Id, eventDTO[0].Id);
            Assert.AreEqual(events[0].EmployeeId, eventDTO[0].EmployeeId);
            Assert.AreEqual(events[0].PetId, eventDTO[0].PetId);
            Assert.AreEqual(events[0].PetServiceId, eventDTO[0].PetServiceId);
            Assert.AreEqual(events[0].EventDate, eventDTO[0].EventDate);
            Assert.AreEqual(events[0].Completed, eventDTO[0].Completed);
            Assert.AreEqual(events[0].Canceled, eventDTO[0].Canceled);

            Assert.AreEqual(events[0].Employee.FullName, eventDTO[0].EmployeeFullName);
            Assert.AreEqual(events[0].Pet.Name, eventDTO[0].PetName);
            Assert.AreEqual(events[0].PetService.ServiceName, eventDTO[0].PetServiceName);
        }

        [Test]
        public async Task GetJobEventById_Success()
        {
            var jobEvent = new JobEvent()
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
                    FullName = "John Doe"
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

            _eventService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(jobEvent);

            var controller = new EventController(_eventService.Object);

            var response = await controller.GetJobEventById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;
            Assert.IsNotNull(okObj);
            Assert.AreEqual(okObj.StatusCode, 200);

            Assert.AreEqual(typeof(EventDTO), okObj.Value.GetType());

            var eventDTO = (EventDTO)okObj.Value;
            Assert.AreEqual(jobEvent.Id, eventDTO.Id);
            Assert.AreEqual(jobEvent.EmployeeId, eventDTO.EmployeeId);
            Assert.AreEqual(jobEvent.PetId, eventDTO.PetId);
            Assert.AreEqual(jobEvent.PetServiceId, eventDTO.PetServiceId);
            Assert.AreEqual(jobEvent.EventDate, eventDTO.EventDate);
            Assert.AreEqual(jobEvent.Completed, eventDTO.Completed);
            Assert.AreEqual(jobEvent.Canceled, eventDTO.Canceled);

            Assert.AreEqual(jobEvent.Employee.FullName, eventDTO.EmployeeFullName);
            Assert.AreEqual(jobEvent.Pet.Name, eventDTO.PetName);
            Assert.AreEqual(jobEvent.PetService.ServiceName, eventDTO.PetServiceName);
        }

        [Test]
        public async Task GetJobEventById_NotFound()
        {
            _eventService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ThrowsAsync(new EntityNotFoundException("test"));

            var controller = new EventController(_eventService.Object);

            var response = await controller.GetJobEventById(1);

            Assert.IsNotNull(response);
            Assert.AreEqual(typeof(NotFoundObjectResult), response.GetType());

            var notFoundObj = (NotFoundObjectResult)response;
            Assert.AreEqual(typeof(string), notFoundObj.Value.GetType());
            Assert.AreEqual("test", notFoundObj.Value.ToString());
        }

        [Test]
        public async Task UpdateJobEvent_Success()
        {
            var updateEvent = new EventDTO()
            {
                EmployeeId = 1,
                EmployeeFullName = "John Doe",
                PetId = 1,
                PetName = "Dog1",
                PetServiceId = 1,
                PetServiceName = "Walk",
                EventDate = DateTime.Now,
                Completed = true,
                Canceled = false
            };

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var controller = new EventController(_eventService.Object);

            var response = await controller.UpdateJobEvent(updateEvent);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdateJobEvent_BadRequest()
        {
            var updateEvent = new EventDTO()
            {
                EmployeeId = 1,
                EmployeeFullName = "John Doe",
                PetId = 1,
                PetName = "Dog1",
                PetServiceId = 1,
                PetServiceName = "Walk",
                EventDate = DateTime.Now,
                Completed = true,
                Canceled = false
            };

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new ArgumentException("test"));

            var controller = new EventController(_eventService.Object);

            var response = await controller.UpdateJobEvent(updateEvent);

            Assert.IsNotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var badRequestObj = (BadRequestObjectResult)response;
            Assert.AreEqual(typeof(string), badRequestObj.Value.GetType());
            Assert.AreEqual("test", badRequestObj.Value.ToString());
        }

        [Test]
        public async Task DeleteEventById_Success()
        {
            _eventService.Setup(e => e.DeleteEventById(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var controller = new EventController(_eventService.Object);

            var response = await controller.DeleteEventById(1);

            Assert.IsNotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());
        }
    }
}
