using System;
using System.Collections.Generic;
using System.Text;

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

            if (string.IsNullOrEmpty(PrimaryPhoneNum))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            if (string.IsNullOrEmpty(Address.AddressLine1))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            if (string.IsNullOrEmpty(Address.City))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            if (string.IsNullOrEmpty(Address.State))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            if (string.IsNullOrEmpty(Address.ZipCode))
            {
                invalidErr.Add("Need a primary phone number.");
            }

            return invalidErr;
        }
    }
}
