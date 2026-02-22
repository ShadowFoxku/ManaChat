namespace ManaChat.API.Clients
{
    public class ManaChatWebClient : ManaChatWebBasedClient
    {
        public override string FriendlyName => "ManaWeb";

        public override string InternalName => "manaweb";

        public override string Description => "ManaChat Web Client";

        public override ManaChatVersion MinimumSupportedVersion => new(1, 0, 0);
    }
}
