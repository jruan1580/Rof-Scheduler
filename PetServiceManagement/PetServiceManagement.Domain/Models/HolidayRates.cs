namespace PetServiceManagement.Domain.Models
{
    public class HolidayRates
    {
        public Holiday Holiday { get; set; }

        public PetService PetService { get; set; }

        public decimal HolidayRate { get; set; }
    }
}
