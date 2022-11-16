using System.Collections.Generic;

namespace PetServiceManagement.API.DTO
{
    public class PetServicesWithTotalPageDTO
    {
        public PetServicesWithTotalPageDTO(List<PetServiceDTO> petServices, int totalPages)
        {
            PetServices = petServices;
            TotalPages = totalPages;
        }

        public List<PetServiceDTO> PetServices { get; set; }

        public int TotalPages { get; set; }
    }
}
