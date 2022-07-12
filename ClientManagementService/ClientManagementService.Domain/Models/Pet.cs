using System;
using System.Collections.Generic;
using System.Text;

namespace ClientManagementService.Domain.Models
{
    public class Pet
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

        public List<string> IsValidPetToCreate()
        {
            var invalidErr = new List<string>();

            if (string.IsNullOrEmpty(Name))
            {
                invalidErr.Add("Need pet's name.");
            }

            if (Weight <= 0)
            {
                invalidErr.Add("Need pet's weight.");
            }

            if (string.IsNullOrEmpty(Dob))
            {
                invalidErr.Add("Need pet's birthday.");
            }

            if (OwnerId <= 0)
            {
                invalidErr.Add("Need owner info.");
            }

            if(BreedId <= 0)
            {
                invalidErr.Add("Need breed info.");
            }

            return invalidErr;
        }
    }
}
