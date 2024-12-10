using BoxServer;
using Microsoft.Extensions.Configuration;
using Seekatar.Tools;
using Xunit.Abstractions;

namespace ApiTest;

public partial class ApiTests : XUnitHttpClientTestBase<ApiTests>
{
    // if you don't use 44300 port, you can change it here with an environment variable
    // ApiTests__URIPREFIX='https://localhost:44673'
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