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
            var (pwordBytes, saltBytes) = PasswordHelpers.HashPassword(request.Password, config.Value.Encryption.Passwords);

            var result = await userService.CreateUser(request.Username, request.Email)
                .BindAsync((user) => userService.UpdateUserPassword(user.Id, pwordBytes, saltBytes));

            if (result.IsFlowing && result.GetValue())
                return Ok("Creation successful! Please log in to continue.");

            return BadRequest($"Unable to create user. {result.GetTear()!.Message}");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = (await userService.GetUserPasswordAndSalt(request.Username))
                .Bind((result) => 
                {
                    return Ritual<bool>.Flow(PasswordHelpers.VerifyPassword(request.Password, result.pw, result.s, config.Value.Encryption.Passwords));
                });
               
            if (res.IsTorn)
                return BadRequest($"Unable to log in. {res.GetTear()!.Message}");

            if (res.IsFlowing && res.GetValue())
                return Ok("Login successful!");

            return BadRequest("Invalid username or password.");
        }
    }
}
