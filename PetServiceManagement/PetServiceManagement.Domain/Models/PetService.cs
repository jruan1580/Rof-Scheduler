using PetServiceManagement.Domain.Constants;
using System.Collections.Generic;

namespace PetServiceManagement.Domain.Models
{
    public class PetService
    {
        private readonly HashSet<string> _supportedTimeUnits = new HashSet<string>() { TimeUnits.SECONDS, TimeUnits.MINUTES, TimeUnits.HOURS };

        public short Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal EmployeeRate { get; set; }

        public int Duration { get; set; } 

        public string TimeUnit { get; set; }

        public string Description { get; set; }

        public string GetValidationFailures()
        {
            var failures = new List<string>();

            AddNameValidationFailure(failures);

            AddPriceValidationFailure(failures);

            AddEmployeeRateValidationFailure(0, 100, failures);

            AddDurationValidationFailure(failures);

            AddTimeUnitValidationFailure(failures);

            return string.Join(",", failures);
        }

        private void AddNameValidationFailure(List<string> failures)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return;
            }
           
            failures.Add("Pet service's name is not provided");
        }

        private void AddPriceValidationFailure(List<string> failures)
        {
            if(Price > 0)
            {
                return;
            }

            failures.Add("Pet service rate must be greater than 0");
        }

        private void AddEmployeeRateValidationFailure(int lowerRange, int upperRange, List<string> failures)
        {
            if(EmployeeRate >= lowerRange && EmployeeRate <= upperRange)
            {
                return;
            }

            failures.Add("Employee rate should be between 0 and 100");
        }

        private void AddDurationValidationFailure(List<string> failures)
        {
            if(Duration > 0)
            {
                return;
            }

            failures.Add("Pet Service Duration must be greater than 0");
        }

        private void AddTimeUnitValidationFailure(List<string> failures)
        {
            if (_supportedTimeUnits.Contains(TimeUnit))
            {
                return;
            }

            failures.Add("Time Unit not supported.");
        }
    }
}
