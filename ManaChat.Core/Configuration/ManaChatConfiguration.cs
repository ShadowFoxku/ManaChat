namespace ManaChat.Core.Configuration
{
    public class ManaChatConfiguration
    {
        public UserConfig Users { get; set; } = new UserConfig();
        public EncryptionSettings Encryption { get; set; } = new EncryptionSettings();
        public TokenSettings TokenSettings { get; set; } = new TokenSettings();
    }
}
