using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PetServiceManagement.Infrastructure.Persistence.Entities
{
    public partial class HolidayRates
    {
        public short Id { get; set; }
        public short PetServiceId { get; set; }
        public DateTime HolidayDate { get; set; }
        public decimal HolidayRate { get; set; }

        public virtual PetServices PetService { get; set; }
    }
}
