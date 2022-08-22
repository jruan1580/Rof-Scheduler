using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Vaccine
    {
        public Vaccine()
        {
            PetToVaccines = new HashSet<PetToVaccine>();
        }

        public short Id { get; set; }
        public short PetTypeId { get; set; }
        public string VaxName { get; set; }

        public virtual PetType PetType { get; set; }
        public virtual ICollection<PetToVaccine> PetToVaccines { get; set; }
    }
}
