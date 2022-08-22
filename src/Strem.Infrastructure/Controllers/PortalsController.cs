using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events.Portals;
using Strem.Core.Portals;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Models;

namespace Strem.Infrastructure.Controllers;

[ApiController]
[Route("portals")]
public class PortalsController : Controller
{
    [HttpGet]
    [Route("{portalId:guid}")]
    public IActionResult View([FromRoute] Guid portalId)
    {
        var portalStore = this.GetService<IPortalStore>();
        var portalData = portalStore.Portals.SingleOrDefault(x => x.Id == portalId);
        if(portalData == null) { return NotFound("No Portal Found For That Id"); }
        
        return View("View", new ViewPortalModel { PortalData = portalData });
    }

    [HttpPost]
    [Route("{portalId:guid}")]
    public IActionResult ButtonPressed([FromRoute] Guid portalId, [FromBody] ButtonPressedModel buttonPressedModel)
    {
        this.PublishAsyncEvent(new ButtonPressedEvent
        {
            ButtonId = buttonPressedModel.ButtonId,
            ButtonName = buttonPressedModel.ButtonName,
            PortalId = buttonPressedModel.PortalId,
            PortalName = buttonPressedModel.PortalName
        });

        return Ok();
    }
}