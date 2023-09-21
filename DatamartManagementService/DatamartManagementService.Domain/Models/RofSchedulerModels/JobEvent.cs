using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
