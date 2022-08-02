using System.Collections.Generic;

namespace ClientManagementService.Domain.Models
{
    public class Pet
    {
        public long Id { get; set; }
        
        public long OwnerId { get; set; }

        public short PetTypeId { get; set; }
        
        public short BreedId { get; set; }
        
        public string Name { get; set; }
        
        public decimal Weight { get; set; }
        
        public string Dob { get; set; }
        
        public string OtherInfo { get; set; }
        
        public Client Owner { get; set; }

        public Breed BreedInfo { get; set; }

        public PetType PetType { get; set; }

        public List<VaccineStatus> Vaccines { get; set; }

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

            if (PetTypeId <= 0)
            {
                invalidErr.Add("Need to specify pet type.");
            }

            if (Vaccines == null || Vaccines.Count == 0)
            {
                invalidErr.Add("Vaccines were not specified for pet.");
            }

            return invalidErr;
        }

        public List<string> IsValidPetToUpdate()
        {
            var invalidErr = new List<string>();

            if (Id <= 0)
            {
                invalidErr.Add($"Invalid Id: {Id}.");
            }

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

            if (BreedId <= 0)
            {
                invalidErr.Add("Need breed info.");
            }

            if (Vaccines == null || Vaccines.Count == 0)
            {
                invalidErr.Add("Vaccines were not specified for pet.");
            }

            return invalidErr;
        }
    }
}
