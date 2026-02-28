namespace ManaChat.Core.Configuration
{
    public class ManaChatConfiguration
    {
        public string InstanceName { get; set; } = string.Empty;
        public UserConfig Users { get; set; } = new UserConfig();
        public EncryptionSettings Encryption { get; set; } = new EncryptionSettings();
        public TokenSettings TokenSettings { get; set; } = new TokenSettings();
    }
}
