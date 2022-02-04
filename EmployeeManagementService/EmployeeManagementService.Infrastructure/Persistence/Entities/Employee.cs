using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementService.Infrastructure.Persistence.Entities
{
    public partial class Employee
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public short FailedLoginAttempts { get; set; }
        public bool FirstLoggedIn { get; set; }
        public bool? Active { get; set; }
    }
}
