using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PetServiceManagement.Infrastructure.Persistence.Entities
{
    public partial class Holidays
    {
        public Holidays()
        {
            HolidayRates = new HashSet<HolidayRates>();
        }

        public short Id { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }

        public virtual ICollection<HolidayRates> HolidayRates { get; set; }
    }
}
