using System.Data;
using CoreEvent = EventManagementService.Domain.Models.JobEvent;
using DbEvent = EventManagementService.Infrastructure.Persistence.Entities.JobEvent;

namespace EventManagementService.Domain.Mappers
{
    public static class EventMapper
    {
        public static CoreEvent ToCoreEvent(DbEvent dbEvent)
        {
            var coreEvent = new CoreEvent();

            coreEvent.Id = dbEvent.Id;
            coreEvent.EmployeeId = dbEvent.EmployeeId;
            coreEvent.PetId = dbEvent.PetId;
            coreEvent.PetServiceId = dbEvent.PetServiceId;
            coreEvent.EventStartTime = dbEvent.EventStartTime;
            coreEvent.EventEndTime = dbEvent.EventEndTime;
            coreEvent.Completed = dbEvent.Completed;
            coreEvent.Canceled = dbEvent.Canceled;

            if(dbEvent.Employee != null)
            {
                coreEvent.Employee = new Models.Employee()
                {
                    Id = dbEvent.EmployeeId,
                    FirstName = dbEvent.Employee.FirstName,
                    LastName = dbEvent.Employee.LastName,
                    Role = dbEvent.Employee.Role
                };
            }

            if(dbEvent.Pet != null)
            {
                coreEvent.Pet = new Models.Pet()
                {
                    Id = dbEvent.PetId,
                    Name = dbEvent.Pet.Name
                };
            }

            if (dbEvent.PetService != null)
            {
                coreEvent.PetService = new Models.PetService()
                {
                    Id = dbEvent.PetServiceId,
                    ServiceName = dbEvent.PetService.ServiceName,
                    Description = dbEvent.PetService.Description,
                    Price = dbEvent.PetService.Price
                };
            }

            return coreEvent;
        }

        public static DbEvent FromCoreEvent(CoreEvent coreEvent)
        {
            var entity = new DbEvent();

            entity.Id = coreEvent.Id;
            entity.EmployeeId = coreEvent.EmployeeId;
            entity.PetId = coreEvent.PetId;
            entity.PetServiceId = coreEvent.PetServiceId;
            entity.EventStartTime = coreEvent.EventStartTime;
            entity.EventEndTime = coreEvent.EventEndTime;
            entity.Completed = coreEvent.Completed;
            entity.Canceled = coreEvent.Canceled;

            return entity;
        }
    }
}
