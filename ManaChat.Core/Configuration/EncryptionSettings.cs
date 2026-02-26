namespace ManaChat.Core.Configuration
{
    public class EncryptionSettings
    {
        public PasswordSettings Passwords { get; set; } = new PasswordSettings();

        public bool Validate()
        {
            return Passwords.Validate();
        }
    }

    public class EncryptionKeyIVPair
    {
        public string Key { get; set; } = string.Empty;
        public string IV { get; set;  } = string.Empty;

        public bool Valid()
        {
            return !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(IV);
        }
    }

    /// <summary>
    /// Make sure to check Argon2id before changing and tweaking these settings! These are a safe default, if you find it's taking a long time
    /// it may be worth adjusting, but otherwise these should be fine for most use cases. The defaults are set to be reasonably secure while 
    /// still being performant on modern hardware. Adjusting these settings can increase security but may also increase the time it takes to 
    /// hash passwords, so it's important to find a balance that works for your application and user base.
    /// </summary>
    public class PasswordSettings
    {
        public int DegreeOfParallelism { get; set; } = 2;
        public int Iterations { get; set; } = 4;
        public int MemorySize { get; set; } = 65536; // 64mb

        public bool Validate()
        {
            return DegreeOfParallelism > 0 && Iterations > 0 && MemorySize > 0;
        }
    }
}
