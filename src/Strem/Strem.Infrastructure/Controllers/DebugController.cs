using Microsoft.AspNetCore.Mvc;

namespace Strem.Infrastructure.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    { return Ok("Works Fine"); }
}