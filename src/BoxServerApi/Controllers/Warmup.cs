using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BoxServer.Controllers;

/// <summary>
/// K8s warmup
/// </summary>
#pragma warning disable CA1825 // Avoid zero-length array allocations. (causes by Produces in lastest nuget)
[Produces("application/json")]
[SwaggerResponse(StatusCodes.Status200OK)]
[SwaggerResponse(StatusCodes.Status204NoContent, "No content")]
[ApiController]
[AllowAnonymous]
public class WarmupController : ControllerBase
{

    /// <summary>
    ///
    /// </summary>
    public WarmupController()
    {
    }

    /// <summary>
    /// Hi!
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("api/warmup")]
    public ActionResult Warmup()
    {
        return Ok();
    }
}