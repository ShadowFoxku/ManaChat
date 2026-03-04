using ManaChat.API.Controllers.Models;
using ManaChat.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class OpenController(IOptions<ManaChatConfiguration> config) : RitualControllerBase
    {   // this controller is primarily for stateless/non-impactful checks. Ensuring logins are valid, getting public config, 
        // and similar, open and safe tasks that often do not need auth. 
        [AllowAnonymous]
        [HttpGet("site/config")]
        public IActionResult GetConfig()
        {
            var chatConfig = config.Value;
            return Ok(new ConfigResponse(chatConfig));
        }

        [HttpGet("auth")]
        public IActionResult GetAuthState()
        {
            return NoContent();
        }

        [HttpGet("health/status")]
        public IActionResult GetStatus()
        {
            return NoContent();
        }
    }
}
