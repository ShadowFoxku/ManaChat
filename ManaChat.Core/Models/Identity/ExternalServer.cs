namespace ManaChat.Core.Models.Identity
{
    public class ExternalServer
    {
        public long Id { get; set; }
        public string FriendlyName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
    }
}
