using System;
using System.Collections.Generic;

namespace EventManagementService.Domain.Models
{
    public class JobEvent
    {
        public int Id { get; set; }

        public long EmployeeId { get; set; }
                
        public long PetId { get; set; }
        
        public short PetServiceId { get; set; }

        public DateTime EventStartTime { get; set; }

        public DateTime EventEndTime { get; set; }

        public bool Completed { get; set; }
        
        public Employee Employee { get; set; }

        public Pet Pet { get; set; }
        
        public PetService PetService { get; set; }

        public List<string> GetValidationErrorsForUpdate()
        {
            var validationErrors = new List<string>();

            if (Id <= 0)
            {
                validationErrors.Add($"Invalid Id: {Id}");
            }

            var remainingPropertyValidationErrors = GetValidationErrorsForBothCreateOrUpdate();
            validationErrors.AddRange(remainingPropertyValidationErrors);

            return validationErrors;
        }

        public List<string> GetValidationErrorsForBothCreateOrUpdate()
        {
            var validationErrors = new List<string>();
            var failedMessageIfValidationResultIsTrue = new Dictionary<string, bool>();

            failedMessageIfValidationResultIsTrue.Add($"Invalid EmployeeId: {EmployeeId}", EmployeeId <= 0);
            failedMessageIfValidationResultIsTrue.Add($"Invalid PetId: {PetId}", PetId <= 0);
            failedMessageIfValidationResultIsTrue.Add($"Invalid PetServiceId: {PetServiceId}", PetServiceId <= 0);
            failedMessageIfValidationResultIsTrue.Add("Please set a start date and time for event.", EventStartTime == null);

            foreach (var failedMessageToValidationResult in failedMessageIfValidationResultIsTrue)
            {
                var validationFailed = failedMessageToValidationResult.Value;
                if (validationFailed)
                {
                    var msg = failedMessageToValidationResult.Key;
                    validationErrors.Add(msg);
                }
            }

            return validationErrors;
        }
    }
}
