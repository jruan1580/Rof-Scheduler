namespace ClientManagementService.API.DTO
{
    public class ClientDTO
    {
        public long Id { get; set; }

        public long CountryId { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PrimaryPhoneNum { get; set; }

        public string SecondaryPhoneNum { get; set; }

        public bool TempPasswordChanged { get; set; }

        public bool IsLocked { get; set; }

        public short FailedLoginAttempts { get; set; }

        public bool IsLoggedIn { get; set; }

        public AddressDTO Address { get; set; }
    }
}
