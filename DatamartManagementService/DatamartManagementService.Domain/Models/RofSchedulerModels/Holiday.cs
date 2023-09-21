using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models.RofSchedulerModels
{
    public class Holiday
    {
        public short Id { get; set; }
        
        public string HolidayName { get; set; }
        
        public short HolidayMonth { get; set; }
        
        public short HolidayDay { get; set; }
    }

}
