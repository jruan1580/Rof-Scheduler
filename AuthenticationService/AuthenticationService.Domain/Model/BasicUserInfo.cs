﻿using AuthenticationService.Infrastructure.ClientManagement.Models;
using AuthenticationService.Infrastructure.EmployeeManagement.Models;

namespace AuthenticationService.Domain.Model
{        
    public class BasicUserInfo
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Role { get; set; }

        public BasicUserInfo MapFromEmployeeLoginResponse(EmployeeLoginResponse resp)
        {
            Id = resp.Id;
            FirstName = resp.FirstName;
            Role = resp.Role;

            return this;
        }

        public BasicUserInfo MapFromClientLoginResponse(ClientLoginResponse resp)
        {
            Id = resp.Id;
            FirstName = resp.FirstName;
            Role = "Client";

            return this;
        }
    }
}
