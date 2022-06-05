using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Pet
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Type { get; set; }
        public decimal Weight { get; set; }
        public string Dob { get; set; }
        public bool BordetellaVax { get; set; }
        public bool Dhppvax { get; set; }
        public bool RabieVax { get; set; }
        public string OtherInfo { get; set; }
        public byte[] Picture { get; set; }

        public virtual Client Owner { get; set; }
    }
}
