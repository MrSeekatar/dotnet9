using BoxServer.Interfaces;
using BoxServer.Repositories;
using Loyal.Core.Database.Utils;
using Loyal.Core.Extensions;
using Loyal.Core.HealthChecks.Checks;
using Loyal.Core.HealthChecks.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BoxServer.Extensions;

internal static class ServiceExtensions
{
    public const string CorsPolicyName = "CorsPolicy";

    internal static IServiceCollection AddDependentServices(this WebApplicationBuilder builder, string appName, int apiVersion = 1)
    {
        var services = builder.Services;

        // TODO Add your dependent services here
        services.AddSingleton<IBoxRepository, BoxRepository>();

        services.AddSwaggerGen(c =>
        {
            if (!File.Exists(Path.Join(Directory.GetCurrentDirectory(), $"{appName}.xml"))) return;

            // unittest not getting a copy of this file for some projects??
            try
            {
                c.SetApiVersion($"v{apiVersion}", $"{appName} - V{apiVersion}")
                    .IncludeComments($"{appName}.xml")
                    .IncludeComments($"{appName}Models.xml")
                    .IncludeComments($"{appName}Models.xml");

                c.CustomSchemaIds(type => type.FullName);
                c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                c.UseAllOfForInheritance();
                c.CustomOperationIds(apiDesc =>
                {
                    // convert method names to readable text for api docs
                    var methodName = apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
                    apiDesc.RelativePath = apiDesc.RelativePath?.Replace("api/", string.Empty, StringComparison.OrdinalIgnoreCase);

                    var result = methodName?.Replace("Client_", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase);

                    return result;
                });

                c.OperationFilter<DisplayOperationFilter>();

                c.SchemaFilter<StringLengthSchemaFilter>();
            }
            catch (FileNotFoundException)
            {
            }
        });

        services.AddCors(options => options.AddPolicy(CorsPolicyName, builder =>
        {
            builder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        }));

        // exclude ones we don't use it
        services.AddLoyalHealthChecks(exclusions: new[] { typeof(RedisHealthCheck) });

        return services;
    }

    internal static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbOptions(builder.Configuration);
        return builder;
    }
}