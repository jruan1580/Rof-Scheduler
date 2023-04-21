using System.Collections.Generic;
using System.Net.Mail;

namespace ClientManagementService.Domain.Models
{
    public class Pet
    {
        public long Id { get; set; }
        
        public long OwnerId { get; set; }

        public short PetTypeId { get; set; }
        
        public short BreedId { get; set; }
        
        public string Name { get; set; }
        
        public decimal Weight { get; set; }
        
        public string Dob { get; set; }
        
        public string OtherInfo { get; set; }
        
        public Client Owner { get; set; }

        public Breed BreedInfo { get; set; }

        public PetType PetType { get; set; }

        public List<VaccineStatus> Vaccines { get; set; }

        public List<string> GetValidationErrorsForUpdate()
        {
            var validationErrors = new List<string>();

            if (Id <= 0)
            {
                validationErrors.Add($"Invalid Id: {Id}.");
            }

            var remainingPropertyValidationErrors = GetValidationErrorsForBothCreateOrUpdate();
            validationErrors.AddRange(remainingPropertyValidationErrors);

            return validationErrors;
        }

        public List<string> GetValidationErrorsForBothCreateOrUpdate()
        {
            var validationErrors = new List<string>();
            var failedMessageIfValidationResultIsTrue = new Dictionary<string, bool>();

            failedMessageIfValidationResultIsTrue.Add("Pet name cannot be empty", string.IsNullOrEmpty(Name));
            failedMessageIfValidationResultIsTrue.Add("Weight cannot be less than 0", Weight <= 0);
            failedMessageIfValidationResultIsTrue.Add("DOB cannot be empty", string.IsNullOrEmpty(Dob));
            failedMessageIfValidationResultIsTrue.Add("Owner cannot be empty", OwnerId <= 0);
            failedMessageIfValidationResultIsTrue.Add("Breed cannot be empty", BreedId <= 0);
            failedMessageIfValidationResultIsTrue.Add("Vaccines were not specified", Vaccines == null || Vaccines.Count == 0);

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
