using System.Text.RegularExpressions;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using BoxServer;
using BoxServer.Extensions;
using Seekatar.Tools;
using Serilog;
using Serilog.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen; // for key vault

// configured in the values.yaml ingress
// hostname: boxserverapi-#{environment}#-#{availabilityZoneLower}#.loyalhealth.internal
Regex hostRegex = new(@"-(?<environment>\w+)-\w+\.loyalhealth\.internal$", RegexOptions.Compiled);

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Host.UseSerilog((ctx, loggerConfig) => loggerConfig.ReadFrom.Configuration(builder.Configuration));
builder.Configuration.AddSharedDevSettings();

// AzureKeyVault support (if enabled in config)
var vaultConfig = builder.Configuration.GetSection(VaultOptions.SectionName).Get<VaultOptions>();
if (vaultConfig?.Enabled ?? false)
{
    Console.WriteLine($"Adding Azure Key Vault to Configuration: {vaultConfig.Uri}");
    #if DEBUG
    var cred = new Azure.Identity.DefaultAzureCredential();
    #else
    var cred = new Azure.Identity.WorkloadIdentityCredential();
    #endif

    try
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(vaultConfig.Uri),
            cred,
            new AzureKeyVaultConfigurationOptions()
            {
                ReloadInterval = vaultConfig.ReloadInterval
            });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding Azure Key Vault: {ex.Message}");
    }
}


builder.AddOptions();
builder.AddDependentServices("BoxServer");
// end added

builder.Services.AddControllers(options =>
    {
    })
    .AddJsonOptions(CustomSerializerApiOptions.SetOptions);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// if needed
// DbConnectionEx.SetupSqlEncryptionWithAzureClientId(app.Services.GetRequiredService<IOptions<DbOptions>>().Value);

app.AddExceptionHandling();

app.UseSwagger(config =>
{
    // use /docs for swagger
    config.RouteTemplate = "docs/{documentName}/docs.json";

    // add a server to the OAS doc to be accurate when deployed
    config.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var environment = "dev";

        // get host as configured in ingress
        var matches =  hostRegex.Matches(httpReq.Host.Value ?? "");
        if (matches.Count > 0)
            environment = matches[0].Groups["environment"].Value;

        var localhost = $"{httpReq.Scheme}://localhost:{httpReq.Host.Port}";
        swagger.Servers = new List<OpenApiServer>
        {
            new() { Url = localhost },
        };
    });
});

app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("v1/docs.json", "BoxServer API Documentation");
    config.RoutePrefix = "docs";
    // if not localhost need to strip out /api from the url
    config.UseRequestInterceptor("(req) => { console.log(req); if (req.url.includes('loyalhealth.com')) { req.url = req.url.replace('api/', ''); } console.log(req); return req; }");
});


app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(ServiceExtensions.CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

internal class VaultOptions
{
    public const string SectionName = "KeyVault";

    public string VaultName { get; set; } = "";
    public string UriTemplate { get; set; } = "https://{0}.vault.azure.net/";
    public int? ReloadIntervalMin { get; set; } = 1;

    public TimeSpan? ReloadInterval => ReloadIntervalMin.HasValue ? TimeSpan.FromMinutes(ReloadIntervalMin.Value) : null;
    public bool Enabled => !string.IsNullOrWhiteSpace(VaultName);
    public string Uri => string.Format(UriTemplate, VaultName);
}