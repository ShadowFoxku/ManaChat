using ManaChat.API.Controllers.Identity.Models;
using ManaChat.API.Helpers;
using ManaChat.Core.Configuration;
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
    public class UserController(IUserService userService, IOptions<ManaChatConfiguration> config) : ControllerBase
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

            if (result.IsFlowing)
                return Ok("Creation successful! Please log in to continue.");

            return BadRequest($"Unable to create user. {result.GetTear()!.Message}");
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
                    if (result.needsReHash)
                    {
                        var newHash = PasswordHelpers.HashPassword(request.Password, config.Value.Encryption.Passwords);
                        await userService.UpdateUserPassword(result.userId, newHash); 
                    }
                        
                    return Ritual<bool>.Flow(result.isValid);
                });
               
            if (res.IsTorn)
                return BadRequest($"Unable to log in. {res.GetTear()!.Message}");

            if (res.IsFlowing && res.GetValue())
                return Ok("Login successful!");

            return BadRequest("Invalid username or password.");
        }
    }
}
