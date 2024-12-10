using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoxServer;

/// <summary>
/// Serializer options
/// </summary>
public static class CustomSerializerOptions
{
    /// <summary>
    ///
    /// </summary>
    public static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };
}