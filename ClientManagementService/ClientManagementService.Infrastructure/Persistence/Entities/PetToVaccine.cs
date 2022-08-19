using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class PetToVaccine
    {
        public long Id { get; set; }
        public long PetId { get; set; }
        public short VaxId { get; set; }
        public bool? Inoculated { get; set; }

        public virtual Pet Pet { get; set; }
        public virtual Vaccine Vax { get; set; }
    }
}
