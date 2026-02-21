namespace ManaChat.Core.Configuration
{
    public class UserConfig
    {
        public UserAccountOptions AccountOptions { get; set; } = new UserAccountOptions();
        public UserIdentityOptions IdentityOptions { get; set; } = new UserIdentityOptions();
    }

    public class UserAccountOptions
    {
        public bool RequireEmail { get; set; } = false;
        public bool RequireSMS { get; set; } = false;
    }

    public class UserIdentityOptions
    {
        public bool AllowMultipleIdentities { get; set; } = true;
        public int? MaxIdentitiesPerUser { get; set; } = null;
    }
}
