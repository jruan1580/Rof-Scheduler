using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagementService.API.DTO
{
    public class PetDTO
    {
        public long Id { get; set; }

        public long OwnerId { get; set; }

        public long BreedId { get; set; }

        public string Name { get; set; }

        public decimal Weight { get; set; }

        public string Dob { get; set; }

        public bool BordetellaVax { get; set; }

        public bool Dhppvax { get; set; }

        public bool RabieVax { get; set; }

        public string OtherInfo { get; set; }

        public byte[] Picture { get; set; }
    }
}
