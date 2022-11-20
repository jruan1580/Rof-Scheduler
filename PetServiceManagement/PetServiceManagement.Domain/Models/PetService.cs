namespace PetServiceManagement.Domain.Models
{
    public class PetService
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal EmployeeRate { get; set; }

        public int Duration { get; set; } 

        public string TimeUnit { get; set; }

        public string Description { get; set; }

    }
}
