using BoxServer;
using Microsoft.Extensions.Configuration;
using Seekatar.Tools;
using Xunit.Abstractions;

namespace ApiTest;

public partial class ApiTests : XUnitHttpClientTestBase<ApiTests>
{
    public ApiTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        DefaultSerializationOptions = CustomSerializerOptions.Options;
        HttpClient.ShouldNotBeNull();

        var configuration = new ConfigurationBuilder()
            .AddSharedDevSettings()
            .AddEnvironmentVariables()
            .Build();

        BasicAuthenticationOptions.SetHttpClientDefault(HttpClient, configuration);
    }
}