using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
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
        public virtual PetServices PetService { get; set; }
    }
}
