using CoreJobEvent = DatamartManagementService.Domain.Models.RofSchedulerModels.JobEvent;
using DbJobEvent = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.JobEvent;
using CoreEmployee = DatamartManagementService.Domain.Models.RofSchedulerModels.Employee;
using DbEmployee = DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities.Employee;

namespace DatamartManagementService.Domain.Mappers.Database
{
    public static class RofSchedulerMappers
    {
        public static CoreJobEvent ToCoreJobEvent(DbJobEvent dbJobEvent)
        {
            var coreEvent = new CoreJobEvent();

            coreEvent.Id = dbJobEvent.Id;
            coreEvent.EmployeeId = dbJobEvent.EmployeeId;
            coreEvent.PetServiceId = dbJobEvent.PetServiceId;
            coreEvent.EventStartTime = dbJobEvent.EventStartTime;
            coreEvent.EventEndTime = dbJobEvent.EventEndTime;
            coreEvent.Completed = dbJobEvent.Completed;

            return coreEvent;
        }

        public static CoreEmployee ToCoreEmployee(DbEmployee dbEmployee)
        {
            var coreEmployee = new CoreEmployee();

            coreEmployee.Id = dbEmployee.Id;
            coreEmployee.FirstName = dbEmployee.FirstName;
            coreEmployee.LastName = dbEmployee.LastName;

            return coreEmployee;
        }
    }
}
