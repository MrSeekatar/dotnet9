using BoxServer.Interfaces;
using BoxServer.Repositories;
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
                c.SwaggerDoc($"v{apiVersion}", new OpenApiInfo
                {
                    Title = $"{appName} - V{apiVersion}",
                    Version = $"v{apiVersion}"
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,$"{appName}.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,$"{appName}Models.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,$"{appName}Models.xml"));

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

        return services;
    }

    internal static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {

        return builder;
    }
}