using System;
using System.Collections.Generic;
using System.Text;

namespace ManaChat.Core.Configuration
{
    public class TokenSettings
    {
        public double DefaultTokenExpiration { get; set; } = 3;
        public TokenExpirationTime DefaultTokenExpirationUnits { get; set; } = TokenExpirationTime.Days;

        /// <summary>
        /// If true, the token's expiration time will be reset on each use, effectively extending its validity as long as it's being used.
        /// If false, the token will expire after the initial time span regardless of usage. <seealso cref="MaxTokenLifetime"/> <seealso cref="MaxTokenLifetimeUnits"/>
        /// define how long a token will slide for before it can no longer be refreshed, even with sliding expiration enabled. 
        /// This is a security measure to prevent tokens from being valid indefinitely due to continuous use.
        /// </summary>
        public bool Sliding { get; set; } = true;

        /// <summary>
        /// The maximum amount of time a token can be valid from the moment it's issued, regardless of sliding expiration.
        /// We recommend setting this to a reasonable value to balance security and user experience. 
        /// This ensures that even if a token is continuously used and refreshed, it will eventually expire and require re-authentication,
        /// which can help mitigate risks associated with long-lived tokens.
        /// </summary>
        public double? MaxTokenLifetime { get; set; } = 1;
        public TokenExpirationTime? MaxTokenLifetimeUnits { get; set; } = TokenExpirationTime.Months;

        public TimeSpan GetExpiryTimeSpan()
        {
            return DefaultTokenExpirationUnits switch
            {
                TokenExpirationTime.Minutes => TimeSpan.FromMinutes(DefaultTokenExpiration),
                TokenExpirationTime.Hours => TimeSpan.FromHours(DefaultTokenExpiration),
                TokenExpirationTime.Days => TimeSpan.FromDays(DefaultTokenExpiration),
                TokenExpirationTime.Months => TimeSpan.FromDays(DefaultTokenExpiration * 30),
                TokenExpirationTime.Years => TimeSpan.FromDays(DefaultTokenExpiration * 365),
                _ => TimeSpan.FromMinutes(DefaultTokenExpiration),
            };
        }

        /// <summary>
        /// If the current token can be extended based on the sliding expiration and maximum token lifetime settings.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool CanRefreshCurrentToken(DateTimeOffset start, DateTimeOffset current)
        {   
            if (!Sliding) return false; // can never renew if not sliding

            if (MaxTokenLifetime.HasValue && MaxTokenLifetimeUnits.HasValue)
            {
                var maxLifetime = MaxTokenLifetimeUnits.Value switch
                {
                    TokenExpirationTime.Minutes => TimeSpan.FromMinutes(MaxTokenLifetime.Value),
                    TokenExpirationTime.Hours => TimeSpan.FromHours(MaxTokenLifetime.Value),
                    TokenExpirationTime.Days => TimeSpan.FromDays(MaxTokenLifetime.Value),
                    TokenExpirationTime.Months => TimeSpan.FromDays(MaxTokenLifetime.Value * 30),
                    TokenExpirationTime.Years => TimeSpan.FromDays(MaxTokenLifetime.Value * 365),
                    _ => TimeSpan.FromMinutes(MaxTokenLifetime.Value),
                };

                var expandedTime = current.Add(GetExpiryTimeSpan());
                return expandedTime - start < maxLifetime;
            }

            return true;
        }
    }

    public enum TokenExpirationTime
    {
        Minutes,
        Hours,
        Days,
        Months,
        Years
    }
}
