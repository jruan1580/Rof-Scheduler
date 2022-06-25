using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Client
    {
        public long Id { get; set; }
        public long CountryId { get; set; }
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
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public virtual Country Country { get; set; }
    }
}
