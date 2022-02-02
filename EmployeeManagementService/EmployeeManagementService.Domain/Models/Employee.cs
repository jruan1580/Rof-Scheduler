using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementService.Domain.Models
{
    public class Employee
    {
        public long Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Username { get; set; }
        
        public byte[] Password { get; set; }
        
        public string Role { get; set; }
        
        public bool FirstLoggedIn { get; set; }
        
        public bool Active { get; set; }
    }
}
