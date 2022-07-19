using System.Collections.Generic;

namespace ClientManagementService.Domain.Models
{
    public class PetsWithTotalPage
    {
        public PetsWithTotalPage(List<Pet> pets, int totalPages)
        {
            Pets = pets;
            TotalPages = totalPages;
        }

        public List<Pet> Pets { get; set; }

        public int TotalPages { get; set; }
    }
}
