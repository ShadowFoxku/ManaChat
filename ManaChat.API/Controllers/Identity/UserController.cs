using ManaChat.API.Controllers.Identity.Models;
using ManaChat.API.Helpers;
using ManaChat.Core.Configuration;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Services;
using ManaFox.Core.Flow;
using ManaFox.Extensions.Flow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController(IUserService userService, IOptions<ManaChatConfiguration> config, IAuthenticatedUserDetails user) : RitualControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(SignUpRequest request)
        {
            var pword = PasswordHelpers.HashPassword(request.Password, config.Value.Encryption.Passwords);
            var email = config.Value.Users.AccountOptions.AcceptEmail ? request.Email : string.Empty;
            var phone = config.Value.Users.AccountOptions.AcceptPhoneNumber ? request.PhoneNumber : string.Empty;

            if (config.Value.Users.AccountOptions.RequireEmail && string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required but was not provided.");

            if (config.Value.Users.AccountOptions.RequirePhoneNumber && string.IsNullOrWhiteSpace(phone))
                return BadRequest("Phone number is required but was not provided.");

            var result = await userService.CreateUser(request.Username, email, phone, pword);

            if (!IsRitualValid(result, message => $"Unable to create user. {message}", out var res))
                return res;

            return Ok("Creation successful! Please log in to continue.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = await (await userService.GetUserPassword(request.Username))
                .Bind((result) => 
                {
                    var isValid = PasswordHelpers.VerifyPassword(request.Password, result.Item2, config.Value.Encryption.Passwords);
                    return Ritual<(bool isValid, bool needsReHash, long userId)>.Flow((isValid.isValid, isValid.reHash, result.Item1));
                })
                .BindAsync(async (result) =>
                {
                    if (result.needsReHash && result.isValid)
                    {
                        var newHash = PasswordHelpers.HashPassword(request.Password, config.Value.Encryption.Passwords);
                        await userService.UpdateUserPassword(result.userId, newHash); 
                    }
                        
                    return Ritual<(bool isValid, long userId)>.Flow((result.isValid, result.userId));
                }).BindAsync(async (result) =>
                {
                    if (result.isValid)
                    {
                        var token = TokenHelpers.GenerateNewToken();
                        var expiry = DateTimeOffset.UtcNow.Add(config.Value.TokenSettings.GetExpiryTimeSpan());
                        var res = await userService.UpdateUserSession(0, result.userId, token.hash, expiry);
                        if (!res.IsFlowing)
                            return Ritual<(string token, DateTimeOffset expiry)>.Tear("Unexpected issue with token assignment. Please try again, or contact support if this persists.");
                        return Ritual<(string token, DateTimeOffset expiry)>.Flow((token.token, expiry));
                    }

                    return Ritual<(string token, DateTimeOffset expiry)>.Tear("Invalid username or password.");
                });

            if (!IsRitualValid(res, message => $"Unable to log in. {message}", out var badRes))
                return badRes;

            var (token, expiry) = res.GetValue();
            if (user.UsesCookies)
            {
                Response.Cookies.Append(Clients.ManaChatWebClient.CookieName, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = expiry
                });

                return Ok(LoginResponse.Success("You are now logged in! Welcome!"));
            }

            return Ok(LoginResponse.Success(token, expiry, "You are now logged in! Please use this token in your header to access resources."));
        }
    }
}
