using ManaChat.API.Controllers.Identity.Models;
using ManaChat.Core.Configuration;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Services;
using ManaFox.Core.Flow;
using ManaFox.Extensions.Flow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

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

            if (!IsRitualValid(result, message => $"Unable to fetch identities. {message}", out var res))
                return res;

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

                if (!IsRitualValid(canCreate, message => $"Unable to create identity. {message}", out var res))
                    return res;

                if (!canCreate.GetValue())
                    return BadRequest("You have reached the identity limit for this instance. Please delete or update existing identities, instead.");
            }

            var identity = await identityService.CreateUserIdentity(authedUser.UserId!.Value, request.Name, request.IsDefault);

            if (!IsRitualValid(identity, message => $"Unable to create identity. {message}", out var result))
                return result;

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
                        return Ritual<bool>.Tear(APITear("Identity not found", HttpStatusCode.NotFound));

                    return Ritual<bool>.Flow(true);
                })
                .BindAsync((_) => identityService.SaveUserIdentity(userId, id, request.Name, request.IsDefault));

            if (!IsRitualValid(res, message => $"Unable to update identity. {message}", out var result))
                return result;

            return Ok(res.GetValue());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteIdentity(long id)
        {
            var userId = authedUser.UserId!.Value;
            var res = await identityService.GetUserIdentity(userId, id)
                .BindAsync((res) =>
                {
                    if (res == null)
                        return Ritual<bool>.Tear(APITear("Identity not found", HttpStatusCode.NotFound));

                    if (res.Default)
                        return Ritual<bool>.Tear(APITear("Can not delete identity that is set as default. Please choose a new default before deleting.", HttpStatusCode.BadRequest));

                    return Ritual<bool>.Flow(true);
                })
                .BindAsync((_) => identityService.DeleteUserIdentity(id));

            if (!IsRitualValid(res, message => $"Unable to delete identity. {message}", out var result))
                return result;

            return NoContent();
        }
    }
}
