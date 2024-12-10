namespace BoxServer.Extensions;

/// <summary>
///
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Add global exception handling
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static  WebApplication AddExceptionHandling(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error-local-development");
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        return app;
    }
}