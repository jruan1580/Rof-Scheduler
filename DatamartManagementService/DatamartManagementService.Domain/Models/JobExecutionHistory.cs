using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models
{
    public class JobExecutionHistory
    {
        public long Id { get; set; }
        
        public string JobType { get; set; }
        
        public DateTime LastDatePulled { get; set; }
    }
}
