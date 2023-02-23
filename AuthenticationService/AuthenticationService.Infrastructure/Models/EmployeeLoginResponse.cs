namespace AuthenticationService.Infrastructure.Models
{
    public class EmployeeLoginResponse : LoginResponse
    {
        public string Role { get; set; }
    }
}
