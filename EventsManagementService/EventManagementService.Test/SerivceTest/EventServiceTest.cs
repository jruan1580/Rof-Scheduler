using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Services;
using EventManagementService.Infrastructure.Persistence;
using EventManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.Test.SerivceTest
{
    [TestFixture]
    public class EventServiceTest
    {
        private Mock<IEventRepository> _eventRepository;

        [SetUp]
        public void Setup()
        {
            _eventRepository = new Mock<IEventRepository>();
        }

        [Test]
        public void AddEvent_InvalidInput()
        {
            var newEvent = new Domain.Models.JobEvent()
            {
                EmployeeId = 0,
                PetId = 0,
                PetServiceId = 0,
                EventStartTime = DateTime.Now,
                Canceled = false,
                Completed = false
            };

            var eventService = new EventService(_eventRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.AddEvent(newEvent));
        }

        [Test]
        public void AddEvent_Duplicate()
        {
            var newEvent = new Domain.Models.JobEvent()
            {
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now,
                Canceled = false,
                Completed = false
            };

            _eventRepository.Setup(e => e.JobEventAlreadyExists(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var eventService = new EventService(_eventRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.AddEvent(newEvent));
        }

        [Test]
        public async Task AddEvent_Success()
        {
            var newEvent = new Domain.Models.JobEvent()
            {
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now,
                Canceled = false,
                Completed = false
            };

            _eventRepository.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var eventService = new EventService(_eventRepository.Object);

            await eventService.AddEvent(newEvent);
            _eventRepository.Verify(e => e.AddEvent(It.IsAny<JobEvent>()), Times.Once);
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_NoEvents()
        {
            _eventRepository.Setup(e => e.GetAllJobEventsByMonthAndYear(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<JobEvent>());

            var eventService = new EventService(_eventRepository.Object);

            var results = await eventService.GetAllJobEventsByMonthAndYear(DateTime.Today);

            Assert.IsEmpty(results);
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
                    EventStartTime = DateTime.Today,
                    EventEndTime = DateTime.Today,
                    Completed = false,
                    Canceled = false,
                    Employee = new Employee()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Role = "Employee"
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

            _eventRepository.Setup(e => e.GetAllJobEventsByMonthAndYear(It.IsAny<DateTime>()))
                .ReturnsAsync(events);

            var eventService = new EventService(_eventRepository.Object);

            var results = await eventService.GetAllJobEventsByMonthAndYear(DateTime.Today);

            Assert.IsNotEmpty(results);
            Assert.AreEqual(1, results[0].EmployeeId);
            Assert.AreEqual(1, results[0].PetId);
            Assert.AreEqual(1, results[0].PetServiceId);
            Assert.AreEqual(DateTime.Today, results[0].EventStartTime);
            Assert.AreEqual(DateTime.Today, results[0].EventEndTime);
            Assert.IsFalse(results[0].Completed);
            Assert.IsFalse(results[0].Canceled);

            Assert.IsNotNull(results[0].Employee);
            Assert.AreEqual(1, results[0].Employee.Id);
            Assert.AreEqual("John", results[0].Employee.FirstName);
            Assert.AreEqual("Doe", results[0].Employee.LastName);

            Assert.IsNotNull(results[0].Pet);
            Assert.AreEqual(1, results[0].Pet.Id);
            Assert.AreEqual("Dog1", results[0].Pet.Name);

            Assert.IsNotNull(results[0].PetService);
            Assert.AreEqual(1, results[0].PetService.Id);
            Assert.AreEqual("Walk", results[0].PetService.ServiceName);
        }

        [Test]
        public void GetJobEventById_DoesNotExist()
        {
            _eventRepository.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync((JobEvent)null);

            var eventService = new EventService(_eventRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => eventService.GetJobEventById(1));
        }

        [Test]
        public async Task GetJobEventById_Success()
        {
            _eventRepository.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(new JobEvent()
                {
                    Id = 1,
                    EmployeeId = 1,
                    PetId = 1,
                    PetServiceId = 1,
                    EventStartTime = DateTime.Today,
                    EventEndTime = DateTime.Today,
                    Completed = false,
                    Canceled = false,
                    Employee = new Employee()
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Role = "Employee"
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

            var eventService = new EventService(_eventRepository.Object);

            var result = await eventService.GetJobEventById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.EmployeeId);
            Assert.AreEqual(1, result.PetId);
            Assert.AreEqual(1, result.PetServiceId);
            Assert.AreEqual(DateTime.Today, result.EventStartTime);
            Assert.AreEqual(DateTime.Today, result.EventEndTime);
            Assert.IsFalse(result.Completed);
            Assert.IsFalse(result.Canceled);

            Assert.IsNotNull(result.Employee);
            Assert.AreEqual(1, result.Employee.Id);
            Assert.AreEqual("John", result.Employee.FirstName);
            Assert.AreEqual("Doe", result.Employee.LastName);

            Assert.IsNotNull(result.Pet);
            Assert.AreEqual(1, result.Pet.Id);
            Assert.AreEqual("Dog1", result.Pet.Name);

            Assert.IsNotNull(result.PetService);
            Assert.AreEqual(1, result.PetService.Id);
            Assert.AreEqual("Walk", result.PetService.ServiceName);
        }

        [Test]
        public void UpdateJobEvent_InvalidEntry()
        {
            var updateEvent = new Domain.Models.JobEvent()
            {
                Id = 1,
                EmployeeId = 0,
                PetId = 0,
                PetServiceId = 0,
                EventStartTime = DateTime.Now
            };

            var eventService = new EventService(_eventRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.UpdateJobEvent(updateEvent));
        }

        [Test]
        public void UpdateJobEvent_Duplicate()
        {
            var updateEvent = new Domain.Models.JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now
            };

            _eventRepository.Setup(e => e.JobEventAlreadyExists(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var eventService = new EventService(_eventRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.UpdateJobEvent(updateEvent));
        }

        [Test]
        public async Task UpdateJobEvent_Success()
        {
            var updateEvent = new Domain.Models.JobEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Now
            };

            _eventRepository.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var eventService = new EventService(_eventRepository.Object);

            await eventService.UpdateJobEvent(updateEvent);
            _eventRepository.Verify(e => e.UpdateJobEvent(It.IsAny<JobEvent>()), Times.Once);
        }
    }
}
