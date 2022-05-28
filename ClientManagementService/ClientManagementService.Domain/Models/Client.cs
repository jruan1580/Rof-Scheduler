using System.Collections.Generic;

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

        public List<string> IsValidClientToCreate()
        {
            var invalidErr = new List<string>();

            if (string.IsNullOrEmpty(FirstName))
            {
                invalidErr.Add("Need first name.");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                invalidErr.Add("Need last name.");
            }

            if (string.IsNullOrEmpty(EmailAddress))
            {
                invalidErr.Add("Need email address.");
            }

            if (string.IsNullOrEmpty(Username))
            {
                invalidErr.Add("Need username.");
            }

            if (string.IsNullOrEmpty(PrimaryPhoneNum))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            if (string.IsNullOrEmpty(Address.AddressLine1))
            {
                invalidErr.Add("Need an Address Line 1.");
            }

            if (string.IsNullOrEmpty(Address.City))
            {
                invalidErr.Add("Need a City.");
            }

            if (string.IsNullOrEmpty(Address.State))
            {
                invalidErr.Add("Need a State.");
            }

            if (string.IsNullOrEmpty(Address.ZipCode))
            {
                invalidErr.Add("Need a Zipcode.");
            }

            return invalidErr;
        }

        public List<string> IsValidClientToUpdate()
        {
            var invalidErrs = new List<string>();

            if (Id <= 0)
            {
                invalidErrs.Add($"Invalid Id: {Id}.");
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                invalidErrs.Add("First name cannot be empty.");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                invalidErrs.Add("Last name cannot be empty.");
            }

            if (string.IsNullOrEmpty(EmailAddress))
            {
                invalidErrs.Add("Email Address cannot be empty.");
            }

            if (string.IsNullOrEmpty(PrimaryPhoneNum))
            {
                invalidErrs.Add("Primary Phone Number cannot be empty.");
            }

            if (string.IsNullOrEmpty(Address.AddressLine1))
            {
                invalidErrs.Add("Address Line 1 cannot be empty.");
            }

            if (string.IsNullOrEmpty(Address.City))
            {
                invalidErrs.Add("City cannot be empty.");
            }

            if (string.IsNullOrEmpty(Address.State))
            {
                invalidErrs.Add("State cannot be empty.");
            }

            if (string.IsNullOrEmpty(Address.ZipCode))
            {
                invalidErrs.Add("Zipcode cannot be empty.");
            }

            return invalidErrs;
        }
    }
}
