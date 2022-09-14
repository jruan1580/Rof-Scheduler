namespace PetServiceManagement.Domain.Models
{
    public class Holiday
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public short HolidayMonth { get; set; }

        public short HolidayDay { get; set; }
    }
}
