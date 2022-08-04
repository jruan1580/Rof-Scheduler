using System.Collections.Generic;

namespace ClientManagementService.API.DTO
{
    public class PetDTO
    {
        public long Id { get; set; }

        public long OwnerId { get; set; }

        public string OwnerFirstName { get; set; }

        public string OwnerLastName { get; set; }

        public short BreedId { get; set; }

        public string BreedName { get; set; }

        public short PetTypeId { get; set; }

        public string PetTypeName { get; set; }

        public string Name { get; set; }

        public decimal Weight { get; set; }

        public string Dob { get; set; }

        public string OtherInfo { get; set; }
        
        public List<PetsVaccineDTO> Vaccines { get; set; }
    }
}
