using BoxServer;
using BoxServer.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Seekatar.Tools;
using unit;
using Xunit.Abstractions;

namespace ApiTest;

public partial class ApiTests : XUnitHttpClientTestBase<ApiTests>
{
    private static MockBoxRepository? _mockRepository;
    private static IConfiguration? _configuration;

    public ApiTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        DefaultSerializationOptions = CustomSerializerOptions.Options;
    }

    protected override HttpClient MakeHttpClient() => MakeHttpClientFromFactory();

    public static HttpClient MakeHttpClientFromFactory()
    {
        if (_mockRepository == null)
        {
            _configuration = new ConfigurationBuilder()
                .AddSharedDevSettings()
                .AddEnvironmentVariables()
                .Build();

            _mockRepository = new MockBoxRepository();
        }

        var ret = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // mock overrides
                services.AddSingleton<IBoxRepository>(_mockRepository!);
            });
        }).CreateClient();

        BasicAuthenticationOptions.SetHttpClientDefault(ret, _configuration!);

        return ret;
    }
}
