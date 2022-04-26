using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.DTO
{
    public class PasswordDTO
    {
        public long Id { get; set; }

        public string NewPassword { get; set; }
    }
}
