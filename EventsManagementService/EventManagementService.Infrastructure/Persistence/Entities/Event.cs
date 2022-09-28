using System;
using System.Collections.Generic;

#nullable disable

namespace EventManagementService.Infrastructure.Persistence.Entities
{
    public partial class Event
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public long ClientId { get; set; }
        public long PetId { get; set; }
        public short PetServiceId { get; set; }
        public short EventMonth { get; set; }
        public short EventDay { get; set; }

        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Pet Pet { get; set; }
    }
}
