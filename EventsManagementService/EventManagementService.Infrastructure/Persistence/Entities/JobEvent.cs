using System;
using System.Collections.Generic;

#nullable disable

namespace EventManagementService.Infrastructure.Persistence.Entities
{
    public partial class JobEvent
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public long PetId { get; set; }
        public short PetServiceId { get; set; }
        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public bool Completed { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Pet Pet { get; set; }
        public virtual PetService PetService { get; set; }
    }
}
