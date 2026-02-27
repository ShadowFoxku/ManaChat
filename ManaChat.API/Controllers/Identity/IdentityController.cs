using ManaChat.API.Controllers.Identity.Models;
using ManaChat.Core.Configuration;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Services;
using ManaFox.Extensions.Flow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/identity")]
    public class IdentityController(IIdentityService identityService, IOptions<ManaChatConfiguration> config, IAuthenticatedUserDetails authedUser) : ControllerBase
    {
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

                if (canCreate.IsTorn)
                    return BadRequest($"Unable to create identity. Please try again later.");

                if (!canCreate.GetValue())
                    return BadRequest("You have reached the identity limit for this instance. Please delete or update existing identities, instead.");
            }

            var identity = await identityService.SaveUserIdentity(authedUser.UserId!.Value, 0, request.Name, request.IsDefault);

            if (identity.IsTorn)
                return BadRequest("Unable to create identity. Please try again later.");

            return Ok(identity);
        }
    }
}
