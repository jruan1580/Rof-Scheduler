using System;

namespace PetServiceManagement.API.DTO
{
    public class HolidayDTO
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public short Month { get; set; }

        public short Day { get; set; }

        public string Date { 
            get 
            {
                var month = (Month < 10) ? $"0{Month}" : Month.ToString();
                var day = (Day < 10) ? $"0{Day}" : Day.ToString();

                return $"{month}/{day}/{DateTime.Now.Year}";
            } 
        }
    }
}
