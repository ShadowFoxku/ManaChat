using ManaChat.Core.Configuration;

namespace ManaChat.API.Controllers.Models
{
    public class ConfigResponse(ManaChatConfiguration source)
    {
        public string InstanceName = source.InstanceName;
        public UserConfigResponse Users = new(source.Users);
    }

    public class UserConfigResponse(UserConfig source)
    {
        public AccountConfigResponse AccountOptions = new(source.AccountOptions);
    }

    public class AccountConfigResponse(UserAccountOptions source)
    {
        public bool RequireEmail = source.RequireEmail;
        public bool AcceptEmail = source.AcceptEmail;
        public bool RequirePhoneNumber = source.RequirePhoneNumber;
        public bool AcceptPhoneNumber = source.AcceptPhoneNumber;
    }
}
