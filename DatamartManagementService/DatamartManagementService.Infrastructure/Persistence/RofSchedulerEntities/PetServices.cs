using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
{
    public partial class PetServices
    {
        public PetServices()
        {
            HolidayRates = new HashSet<HolidayRates>();
            JobEvent = new HashSet<JobEvent>();
        }

        public short Id { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public decimal EmployeeRate { get; set; }
        public int Duration { get; set; }
        public string TimeUnit { get; set; }
        public string Description { get; set; }

        public virtual ICollection<HolidayRates> HolidayRates { get; set; }
        public virtual ICollection<JobEvent> JobEvent { get; set; }
    }
}
