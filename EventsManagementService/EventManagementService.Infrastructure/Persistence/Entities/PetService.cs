using System;
using System.Collections.Generic;

#nullable disable

namespace EventManagementService.Infrastructure.Persistence.Entities
{
    public partial class PetService
    {
        public PetService()
        {
            JobEvents = new HashSet<JobEvent>();
        }

        public short Id { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public decimal EmployeeRate { get; set; }
        public int Duration { get; set; }
        public string TimeUnit { get; set; }
        public string Description { get; set; }

        public virtual ICollection<JobEvent> JobEvents { get; set; }
    }
}
