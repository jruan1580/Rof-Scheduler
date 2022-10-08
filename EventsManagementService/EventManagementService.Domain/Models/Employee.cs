using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Models
{
    public class Employee
    {
        public long Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Role { get; set; }

        public string FullName { get; set; }

        public void SetFullName()
        {
            FullName = $"{FirstName} {LastName}";
        }
    }
}
