using Microsoft.AspNetCore.Mvc;
using OnlineService.Models;

namespace OnlineService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppsController : ControllerBase
{
    [HttpPost("uniqueId")]
    public IActionResult RegisterApp(string uniqueId)
    {
        return Ok();
    }
    [HttpPost]
    public IActionResult NotifyAppEvent([FromBody] AppStarted appEvent)
    {
        return Ok();
    }
}
