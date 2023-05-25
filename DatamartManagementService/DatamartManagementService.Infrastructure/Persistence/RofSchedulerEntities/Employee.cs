using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities
{
    public partial class Employee
    {
        public Employee()
        {
            JobEvent = new HashSet<JobEvent>();
        }

        public long Id { get; set; }
        public long CountryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] Password { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public short FailedLoginAttempts { get; set; }
        public bool TempPasswordChanged { get; set; }
        public bool Status { get; set; }
        public bool? Active { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public virtual ICollection<JobEvent> JobEvent { get; set; }
    }
}
