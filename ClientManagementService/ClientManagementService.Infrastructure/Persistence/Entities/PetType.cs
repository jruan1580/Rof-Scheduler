using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class PetType
    {
        public PetType()
        {
            Breeds = new HashSet<Breed>();
            Pets = new HashSet<Pet>();
            Vaccines = new HashSet<Vaccine>();
        }

        public short Id { get; set; }
        public string PetTypeName { get; set; }

        public virtual ICollection<Breed> Breeds { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }
}
