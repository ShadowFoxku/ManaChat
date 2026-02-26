using Microsoft.AspNetCore.Mvc;

namespace ManaChat.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/identity")]
    public class IdentityController : ControllerBase
    {

        [HttpPost]
        public IActionResult CreateIdentity()
        {
            return Ok();
        }
    }
}
