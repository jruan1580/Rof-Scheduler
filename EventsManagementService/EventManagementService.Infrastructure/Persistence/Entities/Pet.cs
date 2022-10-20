﻿using System;
using System.Collections.Generic;

#nullable disable

namespace EventManagementService.Infrastructure.Persistence.Entities
{
    public partial class Pet
    {
        public Pet()
        {
            JobEvents = new HashSet<JobEvent>();
        }

        public long Id { get; set; }
        public long OwnerId { get; set; }
        public short PetTypeId { get; set; }
        public short BreedId { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public string Dob { get; set; }
        public string OtherInfo { get; set; }

        public virtual ICollection<JobEvent> JobEvents { get; set; }
    }
}
