namespace ManaChat.API.Controllers.Identity.Models
{
    public class IdentityResponse
    {
        public string Name { get; set; }
        public long UserId { get; set; }
        public long Id { get; set; }
        public bool IsDefault { get; set; }
    }

    public class EditIdentityRequest
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
