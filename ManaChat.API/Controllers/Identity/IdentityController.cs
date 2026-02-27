using ManaChat.API.Controllers.Identity.Models;
using ManaChat.Core.Configuration;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Services;
using ManaFox.Core.Flow;
using ManaFox.Extensions.Flow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/identity")]
    public class IdentityController(IIdentityService identityService, IOptions<ManaChatConfiguration> config, IAuthenticatedUserDetails authedUser) : RitualControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMyIdentities()
        {
            var userId = authedUser.UserId!.Value;

            var result = await identityService.GetUserIdentities(userId);

            if (!IsRitualValid(result, out string message))
                return BadRequest($"Unable to fetch Identities. {message}");

            return Ok(result.GetValue());
        }

        [HttpPost]
        public async Task<IActionResult> CreateIdentity(EditIdentityRequest request)
        {
            var identityConfig = config.Value.Users.IdentityOptions;

            if (!identityConfig.AllowMultipleIdentities)
                return BadRequest("Identity registration is disabled on this instance. Please update your default identity.");

            if (identityConfig.MaxIdentitiesPerUser.HasValue)
            {
                var canCreate = (await identityService.GetUserIdentities(authedUser.UserId!.Value))
                    .Map((res) => res.Count < identityConfig.MaxIdentitiesPerUser);

                if (!IsRitualValid(canCreate, out string msg))
                    return BadRequest($"Unable to create identity. {msg}");

                if (!canCreate.GetValue())
                    return BadRequest("You have reached the identity limit for this instance. Please delete or update existing identities, instead.");
            }

            var identity = await identityService.CreateUserIdentity(authedUser.UserId!.Value, request.Name, request.IsDefault);

            if (!IsRitualValid(identity, out string message))
                return BadRequest($"Unable to create identity. {message}");

            return Ok(identity.GetValue());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateIdentity(long id, EditIdentityRequest request)
        {
            var userId = authedUser.UserId!.Value;
            var res = await identityService.GetUserIdentity(userId, id)
                .BindAsync(async (res) =>
                {
                    if (res == null)
                        return Ritual<bool>.Tear("User identity not found", "ID404");

                    return Ritual<bool>.Flow(true);
                })
                .BindAsync((_) => identityService.SaveUserIdentity(userId, id, request.Name, request.IsDefault));

            if (!IsRitualValid(res, out string message))
                return BadRequest($"Unable to update identity. {message}");

            return Ok(res.GetValue());
        }
    }
}
