namespace AuthenticationService.Infrastructure.Shared.Models
{
    public class LogoutResponse
    {
        public LogoutResponse(long userId, bool success)
        {
            UserId = userId;
            LogoutSuccess = success;
        }

        public long UserId { get; set; }

        public bool LogoutSuccess { get; set; }
    }
}
