using System;
using System.Collections.Generic;

namespace PetServiceManagement.Domain.Models
{
    public class Holiday
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public short HolidayMonth { get; set; }

        public short HolidayDay { get; set; }

        public string GetHolidayValidationErrors()
        {
            var errors = new List<string>();

            AddNameValidationError(errors);

            AddDateValidationError(errors);

            return string.Join(",", errors);
        }

        private void AddNameValidationError(List<string> errors)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return;
            }

            errors.Add("Holiday was not provided");
        }

        private void AddDateValidationError(List<string> errors)
        {
            var holidayDate = $"{HolidayMonth}/{HolidayDay}/{DateTime.Now.Year}";

            if (DateTime.TryParse(holidayDate, out var date))
            {
                return;
            }

            errors.Add("Invalid holiday month and day supplied");
        }
    }
}
