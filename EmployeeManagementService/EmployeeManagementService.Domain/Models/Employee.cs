using System.Collections.Generic;

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

        /// <summary>
        /// Validates whether or not all fields are present for employee information update.
        /// </summary>
        /// <returns></returns>
        public List<string> IsValidEmployeeForUpdate()
        {
            var invalidErrors = new List<string>();

            if (Id <= 0)
            {
                invalidErrors.Add($"Invalid Id: {Id}");
            }

            if (string.IsNullOrEmpty(Username))
            {
                invalidErrors.Add("Username cannot be empty");
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                invalidErrors.Add("First name cannot be empty");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                invalidErrors.Add("Last name cannot be empty");
            }

            if (string.IsNullOrEmpty(Role))
            {
                invalidErrors.Add("Role cannot be empty");
            }

            if (string.IsNullOrEmpty(Ssn))
            {
                invalidErrors.Add("SSN cannot be empty");
            }

            return invalidErrors;
        }
    }
}
