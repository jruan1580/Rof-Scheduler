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

        public long Id { get; set; }
        public string BreedName { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
