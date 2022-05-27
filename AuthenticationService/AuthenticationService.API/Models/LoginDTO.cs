using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.API.Models
{
    public class LoginDTO
    {        
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
