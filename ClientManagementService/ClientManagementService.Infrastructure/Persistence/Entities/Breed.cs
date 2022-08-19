using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Breed
    {
        public Breed()
        {
            Pets = new HashSet<Pet>();
        }

        public short Id { get; set; }
        public short PetTypeId { get; set; }
        public string BreedName { get; set; }

        public virtual PetType PetType { get; set; }
        public virtual ICollection<Pet> Pets { get; set; }
    }
}
