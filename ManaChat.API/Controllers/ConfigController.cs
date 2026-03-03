using ManaChat.API.Controllers.Models;
using ManaChat.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class ConfigController(IOptions<ManaChatConfiguration> config) : RitualControllerBase
    {
        [AllowAnonymous]
        [HttpGet("site/config")]
        public IActionResult GetConfig()
        {
            var chatConfig = config.Value;
            return Ok(new ConfigResponse(chatConfig));
        }
    }
}
