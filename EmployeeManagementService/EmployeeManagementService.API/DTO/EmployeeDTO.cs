namespace EmployeeManagementService.API.DTO
{
    public class EmployeeDTO
    {
        public long Id { get; set; }

        public long CountryId { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Ssn { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public bool IsLocked { get; set; }

        public short FailedLoginAttempts { get; set; }

        public bool TempPasswordChanged { get; set; }

        public bool Status { get; set; }

        public bool? Active { get; set; }

        public AddressDTO Address { get; set; }
    }
}
