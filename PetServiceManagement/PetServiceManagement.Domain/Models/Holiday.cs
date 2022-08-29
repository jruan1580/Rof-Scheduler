using System;

namespace PetServiceManagement.Domain.Models
{
    public class Holiday
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public DateTime HolidayDate { get; set; }
    }
}
