using Microsoft.AspNetCore.Mvc;

namespace Strem.Portals.Controllers;

[ApiController]
[Route("portals")]
public class PortalsController : Controller
{
    [HttpGet]
    [Route("{portalId:guid}")]
    public IActionResult View([FromRoute] Guid portalId)
    {
        return View("View", new { PortalId = portalId });
    }
}