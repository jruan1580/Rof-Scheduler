using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace ClientManagementService.Domain.Models
{
    public class Client
    {
        public long Id { get; set; }
        
        public long CountryId { get; set; }

        public string FullName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string EmailAddress { get; set; }

        public string Username { get; set; }

        public byte[] Password { get; set; }

        public string PrimaryPhoneNum { get; set; }

        public string SecondaryPhoneNum { get; set; }

        public bool TempPasswordChanged { get; set; }
        
        public bool IsLocked { get; set; }
        
        public short FailedLoginAttempts { get; set; }
        
        public bool IsLoggedIn { get; set; }
        
        public Address Address { get; set; }

        public void SetFullName()
        {
            FullName = $"{FirstName} {LastName}";
        }

        public void SetAddress(string addressline1, string addressLine2, string city, string state, string zipcode)
        {
            if(Address == null)
            {
                Address = new Address();
            }

            Address.AddressLine1 = addressline1;
            Address.AddressLine2 = addressLine2;
            Address.City = city;
            Address.State = state;
            Address.ZipCode = zipcode;
        }

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

            failedMessageIfValidationResultIsTrue.Add("First name cannot be empty", string.IsNullOrEmpty(FirstName));
            failedMessageIfValidationResultIsTrue.Add("Last name cannot be empty", string.IsNullOrEmpty(LastName));
            failedMessageIfValidationResultIsTrue.Add("Email cannot be empty", string.IsNullOrEmpty(EmailAddress));
            failedMessageIfValidationResultIsTrue.Add("Username cannot be empty", string.IsNullOrEmpty(Username));
            failedMessageIfValidationResultIsTrue.Add("Primary phone number cannot be empty", string.IsNullOrEmpty(PrimaryPhoneNum));

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
