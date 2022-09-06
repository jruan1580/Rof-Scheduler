namespace PetServiceManagement.API.DTO
{
    public class HolidayRateDTO
    {
        public int Id { get; set; }

        public PetServiceDTO PetService { get; set; }

        public HolidayDTO Holiday { get; set; }

        public decimal Rate { get; set; }
    }
}
