namespace EventManagementService.Domain.Models
{
    public class PetService
    {
        public short Id { get; set; }

        public string ServiceName { get; set; }

        public decimal Price { get; set; }
                
        public int Duration { get; set; }
        
        public string TimeUnit { get; set; }
        
        public string Description { get; set; }
    }
}
