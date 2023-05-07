using EventManagementService.DTO;
using System;
using DbEvent = EventManagementService.Infrastructure.Persistence.Entities.JobEvent;
using DomainEvent = EventManagementService.Domain.Models.JobEvent;

namespace EventManagementService.Test
{
    public static class EventCreator
    {
        public static DbEvent GetDbEvent()
        {
            return new DbEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Today,
                EventEndTime = DateTime.Today,
                Completed = false
            };
        }

        public static DomainEvent GetDomainEvent()
        {
            return new DomainEvent()
            {
                Id = 1,
                EmployeeId = 1,
                PetId = 1,
                PetServiceId = 1,
                EventStartTime = DateTime.Today,
                Completed = false,
                Employee = new Domain.Models.Employee()
                {
                    Id = 1,
                    FullName = "John Doe"
                },
                Pet = new Domain.Models.Pet()
                {
                    Id = 1,
                    Name = "Layla"
                },
                PetService = new Domain.Models.PetService()
                {
                    Id = 1,
                    ServiceName = "Walk"
                }
            };
        }

        public static EventDTO GetEventDTO()
        {
            return new EventDTO()
            {
                EmployeeId = 1,
                EmployeeFullName = "John Doe",
                PetId = 1,
                PetName = "Dog1",
                PetServiceId = 1,
                PetServiceName = "Walk",
                EventStartTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Completed = false
            };
        }
    }
}
