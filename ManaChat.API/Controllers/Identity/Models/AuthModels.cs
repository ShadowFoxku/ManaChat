namespace ManaChat.API.Controllers.Identity.Models
{
    public class SignUpRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string State { get; set; }
        public string Token { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public string Message { get; set; }

        public static LoginResponse Fail(string message) => new LoginResponse { State = "Failure", Message = message };
        public static LoginResponse Success(string token, DateTimeOffset expiry, string message) => new LoginResponse
        {
            State = "Success",
            Token = token,
            Expiry = expiry,
            Message = message
        };

        public static LoginResponse Success(string message) => new LoginResponse
        {
            State = "Success",
            Message = message
        };
    }
}
