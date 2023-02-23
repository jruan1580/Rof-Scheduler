using AuthenticationService.Infrastructure.Models;

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

        public BasicUserInfo MapFromClientLoginResponse(LoginResponse resp)
        {
            Id = resp.Id;
            FirstName = resp.FirstName;
            Role = "Client";

            return this;
        }
    }
}
