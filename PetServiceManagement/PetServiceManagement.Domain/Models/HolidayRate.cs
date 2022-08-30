namespace PetServiceManagement.Domain.Models
{
    public class HolidayRate
    {
        public int Id { get; set; }
        public Holiday Holiday { get; set; }

        public PetService PetService { get; set; }

        public decimal Rate { get; set; }
    }
}
