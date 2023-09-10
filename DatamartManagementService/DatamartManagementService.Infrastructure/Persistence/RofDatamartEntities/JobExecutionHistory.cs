using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities
{
    public partial class JobExecutionHistory
    {
        public long Id { get; set; }
        public string JobType { get; set; }
        public DateTime LastDatePulled { get; set; }
    }
}
