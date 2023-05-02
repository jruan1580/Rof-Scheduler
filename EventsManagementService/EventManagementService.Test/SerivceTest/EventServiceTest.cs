using EventManagementService.Domain.Services;
using EventManagementService.Infrastructure.Persistence;
using EventManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.Test.SerivceTest
{
    [TestFixture]
    public class EventServiceTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IEventRetrievalRepository> _eventRetrievalRepository;

        [SetUp]
        public void Setup()
        {
            _eventRepository = new Mock<IEventRepository>();
            _eventRetrievalRepository = new Mock<IEventRetrievalRepository>();
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
                Completed = false
            };

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

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
                Completed = false
            };

            _eventRetrievalRepository.Setup(e => e.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

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
                Completed = false
            };

            _eventRepository.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

            await eventService.AddEvent(newEvent);
            _eventRepository.Verify(e => e.AddEvent(It.IsAny<JobEvent>()), Times.Once);
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

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

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

            _eventRetrievalRepository.Setup(e => e.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

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

            _eventRetrievalRepository.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(EventCreator.GetDbEvent());

            _eventRepository.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var eventService = new EventService(_eventRepository.Object, _eventRetrievalRepository.Object);

            await eventService.UpdateJobEvent(updateEvent);
            _eventRepository.Verify(e => e.UpdateJobEvent(It.IsAny<JobEvent>()), Times.Once);
        }
    }
}
