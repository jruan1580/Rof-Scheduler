namespace ClientManagementService.Domain.Models
{
    public class Breed
    {
        public short Id { get; set; }
        
        public string BreedName { get; set; }
        
        public short PetTypeId { get; set; }
    }
}
