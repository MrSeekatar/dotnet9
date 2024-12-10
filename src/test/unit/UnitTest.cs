
namespace unit;


public class UnitTest
{
    public UnitTest()
    {
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", "InstrumentationKey=TODO-replacethis;IngestionEndpoint=https://southcentralus-3.in.applicationinsights.azure.com/;LiveEndpoint=https://southcentralus.livediagnostics.monitor.azure.com/");
    }

    [Fact]
    public void AddMoreTest()
    {
        1.ShouldBe(1);
    }
}