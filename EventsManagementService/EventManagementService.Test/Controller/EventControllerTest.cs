using EventManagementService.Domain.Models;
using EventManagementService.DTO;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventManagementService.Test.Controller
{
    [TestFixture]
    public class EventControllerTest : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/Event";

        private readonly string _exceptionMsg = "Test Exception Message";

        [Test]
        public async Task AddEvent_Success()
        {
            var newEvent = EventCreator.GetEventDTO();

            _eventService.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.Created);
        }

        [Test]
        public async Task AddEvent_BadRequest()
        {
            var newEvent = new EventDTO()
            {
                EmployeeId = 0,
                EmployeeFullName = "",
                PetId = 0,
                PetName = "",
                PetServiceId = 0,
                PetServiceName = "",
                EventStartTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Completed = false,
            };

            _eventService.Setup(e => e.AddEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_Success()
        {
            var events = new List<JobEvent>()
            {
                EventCreator.GetDomainEvent()
            };

            _eventRetrievalService.Setup(e => 
                e.GetAllJobEventsByMonthAndYear(It.IsAny<int>(), 
                    It.IsAny<int>()))
            .ReturnsAsync(events);

            var response = await SendRequest("Administrator", HttpMethod.Get,$"{_baseUrl}?month=1&year=2023");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetAllJobEventsByMonthAndYear_InternalServerError()
        {
            _eventRetrievalService.Setup(e =>
                e.GetAllJobEventsByMonthAndYear(It.IsAny<int>(),
                    It.IsAny<int>()))
            .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?month=1&year=2023");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetJobEventById_Success()
        {
            _eventRetrievalService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ReturnsAsync(EventCreator.GetDomainEvent());

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetJobEventById_NotFound()
        {
            _eventRetrievalService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ThrowsAsync(new EntityNotFoundException("Event"));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _eventNotFoundMessage);
        }

        [Test]
        public async Task GetJobEventById_InternalServerError()
        {
            _eventRetrievalService.Setup(e => e.GetJobEventById(It.IsAny<int>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateJobEvent_Success()
        {
            var updateEvent = EventCreator.GetEventDTO();
            updateEvent.Id = 1;

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Put, _baseUrl, ConvertObjectToStringContent(updateEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateJobEvent_NotFound()
        {
            var updateEvent = EventCreator.GetEventDTO();
            updateEvent.Id = 1;

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new EntityNotFoundException("Event"));

            var response = await SendRequest("Administrator", HttpMethod.Put, _baseUrl, ConvertObjectToStringContent(updateEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _eventNotFoundMessage);
        }

        [Test]
        public async Task UpdateJobEvent_BadRequest()
        {
            var updateEvent = EventCreator.GetEventDTO();
            updateEvent.Id = 1;

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Put, _baseUrl, ConvertObjectToStringContent(updateEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateJobEvent_InternalServerError()
        {
            var updateEvent = EventCreator.GetEventDTO();
            updateEvent.Id = 1;

            _eventService.Setup(e => e.UpdateJobEvent(It.IsAny<JobEvent>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Put, _baseUrl, ConvertObjectToStringContent(updateEvent));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeleteEventById_Success()
        {
            _eventService.Setup(e => e.DeleteEventById(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task DeleteEventById_NotFound()
        {
            _eventService.Setup(e => e.DeleteEventById(It.IsAny<int>()))
                .ThrowsAsync(new EntityNotFoundException("Event"));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _eventNotFoundMessage);
        }

        [Test]
        public async Task DeleteEventById_BadRequest()
        {
            _eventService.Setup(e => e.DeleteEventById(It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeleteEventById_InternalServerError()
        {
            _eventService.Setup(e => e.DeleteEventById(It.IsAny<int>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }
    }
}
