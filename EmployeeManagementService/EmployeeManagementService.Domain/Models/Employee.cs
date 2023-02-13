using System;
using System.Collections.Generic;

namespace EmployeeManagementService.Domain.Models
{
    public class Employee
    {
        //private string _notSantizedSSN;

        public long Id { get; set; }

        public long CountryId { get; set; }

        public string FullName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Ssn { get; set; }
        
        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public byte[] Password { get; set; }
        
        public string Role { get; set; }
        
        public bool IsLocked { get; set; }
        
        public short FailedLoginAttempts { get; set; }

        public bool TempPasswordChanged { get; set; }

        public bool Status { get; set; }
        
        public bool? Active { get; set; }

        public Address Address { get; set; }

        //public string GetNotSanitizedSSN()
        //{
        //    return _notSantizedSSN;
        //}

        ///// <summary>
        ///// Want to set ssn to ***-**-NNNN
        ///// </summary>
        ///// <param name="ssn"></param>
        //public void SetSSN(string ssn)
        //{
        //    _notSantizedSSN = ssn;

        //    if (ssn != null)
        //    {
        //        var lastFourDigit = ssn.Substring(7);

        //        Ssn = $"***-**-{lastFourDigit}";
        //    }           
        //}

        public void SetFullName()
        {
            FullName = $"{FirstName} {LastName}"; 
        }

        public void SetAddress(string addressline1, string addressLine2, string city, string state, string zipcode)
        {
            if (Address == null)
            {
                Address = new Address();
            }

            Address.AddressLine1 = addressline1;
            Address.AddressLine2 = addressLine2;
            Address.City = city;
            Address.State = state;
            Address.ZipCode = zipcode;
        }

        /// <summary>
        /// Validates whether or not all fields are present for employee information update.
        /// </summary>
        /// <returns></returns>
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

        public List<string> GetValidationErrorsForCreate()
        {
            var validationErrors = new List<string>();
                    
            if (string.IsNullOrEmpty(Role))
            {
                validationErrors.Add("Role cannot be empty");
            }

            var remainingPropertyValidationErrors = GetValidationErrorsForBothCreateOrUpdate();
            validationErrors.AddRange(remainingPropertyValidationErrors);

            return validationErrors;
        }

        private List<string> GetValidationErrorsForBothCreateOrUpdate()
        {
            var validationErrors = new List<string>();
            var failedMessageIfValidationResultIsTrue = new Dictionary<string, bool>();

            failedMessageIfValidationResultIsTrue.Add("Username cannot be empty", string.IsNullOrEmpty(Username));
            failedMessageIfValidationResultIsTrue.Add("First name cannot be empty", string.IsNullOrEmpty(FirstName));
            failedMessageIfValidationResultIsTrue.Add("Last name cannot be empty", string.IsNullOrEmpty(LastName));
            failedMessageIfValidationResultIsTrue.Add("SSN cannot be empty", string.IsNullOrEmpty(Ssn));
            failedMessageIfValidationResultIsTrue.Add("Phone number cannot be empty", string.IsNullOrEmpty(PhoneNumber));
            failedMessageIfValidationResultIsTrue.Add("Email cannot be empty", string.IsNullOrEmpty(Email));

            foreach(var failedMessageToValidationResult in failedMessageIfValidationResultIsTrue)
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
