using Loyal.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BoxServer.Controllers;

/// <summary>
/// Called when an error occurs
/// </summary>
/// <remarks>
/// if (app.Environment.IsDevelopment())
/// {
///     app.UseExceptionHandler("/error-local-development");
/// }
/// else
/// {
///     app.UseExceptionHandler("/error");
/// }
/// See
/// https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors
/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
/// </remarks>
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError()
    {
        return Problem();
    }

    /// <summary>
    /// For development, add on stack trace to the details
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-local-development")]
    public IActionResult HandleDevError([FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        if (exceptionHandlerFeature.Error is ProblemDetailsException pd)
        {
            return Problem(pd.Detail, instance: "", pd.Status, pd.Title, pd.ProblemType);
        }
        else
        {
            return Problem(
                title: "An unhandled exception has occurred while executing the request (in Error Controller)!",
                detail: exceptionHandlerFeature.Error.Message);
        }
    }
}