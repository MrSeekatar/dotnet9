using BoxServer;
using BoxServer.Extensions;
using Seekatar.Tools;
using Serilog;
using Serilog.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
builder.Configuration.AddSharedDevSettings();

builder.AddOptions();
builder.AddDependentServices("BoxServer");

builder.Services.AddControllers()
    .AddJsonOptions((options)=>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi("v1"); // ASP.NET 9

var app = builder.Build();

// ASP.NET 9 add map and Scalar. The documentName is the value passed into the AddOpenApi method above
app.MapOpenApi("/docs/{documentName}/docs.json")
    .CacheOutput();

app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/docs/{documentName}/docs.json");
});

app.AddExceptionHandling();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(ServiceExtensions.CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapStaticAssets(); // ASP.NET 9

app.Run();
