﻿using EventManagementService.API.DTO;
using EventManagementService.Domain.Models;
using System;

namespace EventManagementService.API.DtoMapper
{
    public static class EventDTOMapper
    {
        public static EventDTO ToDTOEvent(JobEvent coreEvent)
        {
            if(coreEvent == null)
            {
                return null;    
            }

            var dtoEvent = new EventDTO();

            dtoEvent.Id = coreEvent.Id;
            dtoEvent.EmployeeId = coreEvent.EmployeeId;
            dtoEvent.PetId = coreEvent.PetId;
            dtoEvent.PetServiceId = coreEvent.PetServiceId;
            dtoEvent.EventStartTime = coreEvent.EventStartTime.ToString("yyyy-MM-ddTHH:mm:ss");
            dtoEvent.EventEndTime = coreEvent.EventEndTime.ToString("yyyy-MM-ddTHH:mm:ss");
            dtoEvent.Completed = coreEvent.Completed;

            if(coreEvent.Employee != null)
            {
                dtoEvent.EmployeeFullName = coreEvent.Employee.FullName;
            }

            if (coreEvent.Pet != null)
            {
                dtoEvent.PetName = coreEvent.Pet.Name;
            }

            if(coreEvent.PetService != null)
            {
                dtoEvent.PetServiceName = coreEvent.PetService.ServiceName;
            }

            return dtoEvent;
        }

        public static JobEvent FromDTOEvent(EventDTO eventDTO)
        {
            if(eventDTO == null)
            {
                return null;
            }

            var date = new DateTime();

            if (!DateTime.TryParse(eventDTO.EventStartTime, out date))
            {
                throw new ArgumentException("Event date is not a date.");
            }

            var jobEvent = new JobEvent();

            jobEvent.Id = eventDTO.Id;
            jobEvent.EmployeeId = eventDTO.EmployeeId;
            jobEvent.PetId = eventDTO.PetId;
            jobEvent.PetServiceId = eventDTO.PetServiceId;
            jobEvent.EventStartTime = date;
            jobEvent.Completed = eventDTO.Completed;
            jobEvent.Employee = new Employee() { Id = eventDTO.EmployeeId, FullName = eventDTO.EmployeeFullName };
            jobEvent.Pet = new Pet() { Id = eventDTO.PetId, Name = eventDTO.PetName };
            jobEvent.PetService = new PetService() { Id = eventDTO.PetServiceId, ServiceName = eventDTO.PetServiceName };

            return jobEvent;
        }
    }
}
