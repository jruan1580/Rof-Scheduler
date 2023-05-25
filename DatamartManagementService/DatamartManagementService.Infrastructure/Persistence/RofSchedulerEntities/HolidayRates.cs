using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
{
    public partial class HolidayRates
    {
        public int Id { get; set; }
        public short PetServiceId { get; set; }
        public short HolidayId { get; set; }
        public decimal HolidayRate { get; set; }

        public virtual Holidays Holiday { get; set; }
        public virtual PetServices PetService { get; set; }
    }
}
