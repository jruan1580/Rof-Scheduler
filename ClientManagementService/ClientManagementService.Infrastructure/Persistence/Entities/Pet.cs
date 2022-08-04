using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Pet
    {
        public Pet()
        {
            PetToVaccines = new HashSet<PetToVaccine>();
        }

        public long Id { get; set; }
        public long OwnerId { get; set; }
        public short PetTypeId { get; set; }
        public short BreedId { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public string Dob { get; set; }
        public string OtherInfo { get; set; }

        public virtual Breed Breed { get; set; }
        public virtual Client Owner { get; set; }
        public virtual PetType PetType { get; set; }
        public virtual ICollection<PetToVaccine> PetToVaccines { get; set; }
    }
}
