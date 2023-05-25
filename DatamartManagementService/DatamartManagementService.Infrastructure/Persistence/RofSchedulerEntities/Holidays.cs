using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
{
    public partial class Holidays
    {
        public Holidays()
        {
            HolidayRates = new HashSet<HolidayRates>();
        }

        public short Id { get; set; }
        public string HolidayName { get; set; }
        public short HolidayMonth { get; set; }
        public short HolidayDay { get; set; }

        public virtual ICollection<HolidayRates> HolidayRates { get; set; }
    }
}
