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

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetJobEventById_Success()
        {
            _eventService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(new JobEvent()
                {
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
                });

            var controller = new EventController(_eventService.Object);

            var response = await controller.GetJobEventById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task UpdateJobEvent_Success()
        {

        }
    }
}
