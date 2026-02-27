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
        public string State { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset Expiry { get; set; }
        public string Message { get; set; } = string.Empty;

        public static LoginResponse Fail(string message) => new() { State = "Failure", Message = message };
        public static LoginResponse Success(string token, DateTimeOffset expiry, string message) => new()
        {
            State = "Success",
            Token = token,
            Expiry = expiry,
            Message = message
        };

        public static LoginResponse Success(string message) => new() { State = "Success", Message = message };
    }
}
