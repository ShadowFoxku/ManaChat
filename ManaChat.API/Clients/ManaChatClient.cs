namespace ManaChat.API.Clients
{
    public abstract class ManaChatClient
    {
        public abstract string FriendlyName { get; }
        public abstract string InternalName { get; }
        public abstract string Description { get; }
        public abstract string GetClientToken(HttpRequest request);
        public abstract ManaChatVersion MinimumSupportedVersion { get; }
    }

    public abstract class ManaChatWebBasedClient : ManaChatClient
    {
        public override string GetClientToken(HttpRequest request)
        {
            return request.Cookies.TryGetValue("X-ManaChat-Client-Token", out var token) ? token.ToString() : string.Empty;
        }
    }

    public class ManaChatVersion(int major, int minor, int patch)
    {
        public int Major { get; } = major;
        public int Minor { get; } = minor;
        public int Patch { get; } = patch;

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
