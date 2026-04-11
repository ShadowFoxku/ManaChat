using ManaFox.Security.Passwords;

namespace ManaChat.Core.Configuration
{
    public class EncryptionSettings
    {
        public PasswordSettings Passwords { get; set; } = new PasswordSettings();
    }

    public class EncryptionKeyIVPair
    {
        public string Key { get; set; } = string.Empty;
        public string IV { get; set;  } = string.Empty;
    }
}
