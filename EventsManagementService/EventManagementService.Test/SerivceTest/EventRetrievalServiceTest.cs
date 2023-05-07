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
    public class EventRetrievalServiceTest
    {
        [Test]
        public async Task GetAllJobEvents_NoEvents()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            eventRetrievalRepo.Setup(e => e.GetAllJobEvents())
                .ReturnsAsync(new List<JobEvent>());

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            var results = await eventService.GetAllJobEvents();

            Assert.IsEmpty(results);
        }

        [Test]
        public async Task GetAllJobEvents_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            var events = new List<JobEvent>()
            {
                EventCreator.GetDbEvent()
            };

            eventRetrievalRepo.Setup(e => e.GetAllJobEvents())
                .ReturnsAsync(events);

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            var results = await eventService.GetAllJobEvents();

            AssertEventExpectedEqualsActualValues(results[0]);
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_NoEvents()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            eventRetrievalRepo.Setup(e => e.GetAllJobEventsByMonthAndYear(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<JobEvent>());

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            var results = await eventService.GetAllJobEventsByMonthAndYear(DateTime.Today.Month, DateTime.Today.Year);

            Assert.IsEmpty(results);
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            var events = new List<JobEvent>()
            {
                EventCreator.GetDbEvent()
            };

            eventRetrievalRepo.Setup(e => e.GetAllJobEventsByMonthAndYear(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(events);

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            var results = await eventService.GetAllJobEventsByMonthAndYear(DateTime.Today.Month, DateTime.Today.Year);

            AssertEventExpectedEqualsActualValues(results[0]);
        }

        [Test]
        public void GetJobEventById_NotFound()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            eventRetrievalRepo.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync((JobEvent)null);

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => eventService.GetJobEventById(1));
        }

        [Test]
        public async Task GetJobEventById_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();

            eventRetrievalRepo.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(EventCreator.GetDbEvent());

            var eventService = new EventRetrievalService(eventRetrievalRepo.Object);

            var result = await eventService.GetJobEventById(1);

            AssertEventExpectedEqualsActualValues(result);
        }

        private void AssertEventExpectedEqualsActualValues(Domain.Models.JobEvent jobEvent)
        {
            Assert.IsNotNull(jobEvent);
            Assert.AreEqual(1, jobEvent.EmployeeId);
            Assert.AreEqual(1, jobEvent.PetId);
            Assert.AreEqual(1, jobEvent.PetServiceId);
            Assert.AreEqual(DateTime.Today, jobEvent.EventStartTime);
            Assert.AreEqual(DateTime.Today, jobEvent.EventEndTime);
            Assert.IsFalse(jobEvent.Completed);
        }
    }
}
