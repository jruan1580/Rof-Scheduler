using EventManagementService.Domain.Services;
using EventManagementService.Infrastructure.Persistence;
using EventManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EventManagementService.Test.SerivceTest
{
    [TestFixture]
    public class EventUpsertServiceTest
    {
        [Test]
        public void AddEvent_InvalidInput()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var newEvent = new Domain.Models.JobEvent()
            {
                EmployeeId = 0,
                PetId = 0,
                PetServiceId = 0,
                EventStartTime = DateTime.Now,
                Completed = false
            };

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.AddEvent(newEvent));
        }

        [Test]
        public void AddEvent_PetOrEmployeeAlreadyScheduledForThisTime()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var newEvent = EventCreator.GetDomainEvent();

            eventRetrievalRepo.Setup(e => 
                e.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(It.IsAny<int>(), 
                    It.IsAny<long>(), 
                    It.IsAny<long>(), 
                    It.IsAny<DateTime>()))
            .ReturnsAsync(true);

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.AddEvent(newEvent));
        }

        [Test]
        public async Task AddEvent_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var newEvent = EventCreator.GetDomainEvent();

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            await eventService.AddEvent(newEvent);

            eventUpsertRepo.Verify(e => 
                e.AddEvent(It.Is<JobEvent>(e => e.Id == newEvent.Id &&
                e.EmployeeId == newEvent.EmployeeId &&
                e.PetId == newEvent.PetId &&
                e.PetServiceId == newEvent.PetServiceId &&
                e.EventStartTime == DateTime.Today)), 
            Times.Once);
        }

        [Test]
        public void UpdateJobEvent_InvalidEntry()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var updateEvent = new Domain.Models.JobEvent()
            {
                Id = 1,
                EmployeeId = 0,
                PetId = 0,
                PetServiceId = 0,
                EventStartTime = DateTime.Now
            };

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.UpdateJobEvent(updateEvent));
        }

        [Test]
        public void UpdateJobEvent_PetOrEmployeeAlreadyScheduledForThisTime()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var updateEvent = EventCreator.GetDomainEvent();

            eventRetrievalRepo.Setup(e => 
                e.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(It.IsAny<int>(), 
                It.IsAny<long>(), 
                It.IsAny<long>(), 
                It.IsAny<DateTime>()))
            .ReturnsAsync(true);

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            Assert.ThrowsAsync<ArgumentException>(() => eventService.UpdateJobEvent(updateEvent));
        }

        [Test]
        public async Task UpdateJobEvent_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            var updateEvent = EventCreator.GetDomainEvent();

            eventRetrievalRepo.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(EventCreator.GetDbEvent());

            eventRetrievalRepo.Setup(e =>
                e.DoesJobEventAtThisTimeAlreadyExistsForPetOrEmployee(It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<long>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(false);

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            await eventService.UpdateJobEvent(updateEvent);

            eventUpsertRepo.Verify(e => e.UpdateJobEvent(It.Is<JobEvent>(e => e.Id == updateEvent.Id &&
                e.EmployeeId == updateEvent.EmployeeId &&
                e.PetId == updateEvent.PetId &&
                e.PetServiceId == updateEvent.PetServiceId &&
                e.EventStartTime == DateTime.Today)), Times.Once);
        }

        [Test]
        public void DeleteEventById_NotFound()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            eventRetrievalRepo.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync((JobEvent)null);

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            eventUpsertRepo.Verify(e => e.DeleteJobEventById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteEventById_Success()
        {
            var eventRetrievalRepo = new Mock<IEventRetrievalRepository>();
            var eventUpsertRepo = new Mock<IEventUpsertRepository>();

            eventRetrievalRepo.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(EventCreator.GetDbEvent());

            var eventService = new EventUpsertService(eventUpsertRepo.Object, eventRetrievalRepo.Object);

            await eventService.DeleteEventById(1);

            eventUpsertRepo.Verify(e => e.DeleteJobEventById(It.Is<int>(id => id == 1)), Times.Once);
        }
    }
}
