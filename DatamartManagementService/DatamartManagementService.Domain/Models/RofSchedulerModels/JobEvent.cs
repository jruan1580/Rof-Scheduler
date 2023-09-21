using System;

namespace DatamartManagementService.Domain.Models.RofSchedulerModels
{
    public class JobEvent
    {
        public int Id { get; set; }
        
        public long EmployeeId { get; set; }
                
        public short PetServiceId { get; set; }
        
        public DateTime EventStartTime { get; set; }
        
        public DateTime EventEndTime { get; set; }
        
        public bool Completed { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
    }
}
