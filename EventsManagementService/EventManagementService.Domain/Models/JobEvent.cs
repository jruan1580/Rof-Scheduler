using System;
using System.Collections.Generic;

namespace EventManagementService.Domain.Models
{
    public class JobEvent
    {
        public int Id { get; set; }

        public long EmployeeId { get; set; }
                
        public long PetId { get; set; }
        
        public short PetServiceId { get; set; }

        public DateTime EventStartTime { get; set; }

        public DateTime EventEndTime { get; set; }

        public bool Completed { get; set; }
        
        public Employee Employee { get; set; }

        public Pet Pet { get; set; }
        
        public PetService PetService { get; set; }

        public List<string> IsValidEventToCreate()
        {
            var invalidErr = new List<string>();

            if (EmployeeId <= 0)
            {
                invalidErr.Add($"Invalid EmployeeId: {EmployeeId}.");
            }

            if (PetId <= 0)
            {
                invalidErr.Add($"Invalid PetId: {PetId}.");
            }

            if (PetServiceId <= 0)
            {
                invalidErr.Add($"Invalid PetServiceId: {PetServiceId}.");
            }

            if(EventStartTime == null)
            {
                invalidErr.Add("Please set a start date and time for event.");
            }

            return invalidErr;
        }

        public List<string> IsValidEventToUpdate()
        {
            var invalidErr = new List<string>();

            if(Id <= 0)
            {
                invalidErr.Add($"Invalid Id: {Id}.");
            }

            if (EmployeeId <= 0)
            {
                invalidErr.Add($"Invalid EmployeeId: {EmployeeId}.");
            }

            if (PetId <= 0)
            {
                invalidErr.Add($"Invalid PetId: {PetId}.");
            }

            if (PetServiceId <= 0)
            {
                invalidErr.Add($"Invalid PetServiceId: {PetServiceId}.");
            }

            if (EventStartTime == null)
            {
                invalidErr.Add("Please set a start date and time for event.");
            }

            return invalidErr;
        }
    }
}
