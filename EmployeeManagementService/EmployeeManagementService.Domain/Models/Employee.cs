namespace EmployeeManagementService.Domain.Models
{
    public class Employee
    {
        private string _notSantizedSSN;

        public long Id { get; set; }

        public string FullName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Ssn { get; set; }
        
        public string Username { get; set; }
        
        public byte[] Password { get; set; }
        
        public string Role { get; set; }
        
        public bool IsLocked { get; set; }
        
        public short FailedLoginAttempts { get; set; }

        public bool TempPasswordChanged { get; set; }

        public bool Status { get; set; }
        
        public bool? Active { get; set; }

        public string GetNotSanitizedSSN()
        {
            return _notSantizedSSN;
        }

        /// <summary>
        /// Want to set ssn to ***-**-NNNN
        /// </summary>
        /// <param name="ssn"></param>
        public void SetSSN(string ssn)
        {
            _notSantizedSSN = ssn;

            if (ssn != null)
            {
                var lastFourDigit = ssn.Substring(7);

                Ssn = $"***-**-{lastFourDigit}";
            }           
        }

        public void SetFullName()
        {
            FullName = $"{FirstName} {LastName}"; 
        }
    }
}
