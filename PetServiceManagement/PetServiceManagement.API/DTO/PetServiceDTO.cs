namespace PetServiceManagement.API.DTO
{
    public class PetServiceDTO
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public decimal EmployeeRate { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public string TimeUnit { get; set; }
    }
}
