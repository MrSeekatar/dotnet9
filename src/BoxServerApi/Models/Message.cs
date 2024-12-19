using System.Text.Json.Serialization;

namespace BoxServer.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(TypedMessage), nameof(TypedMessage))]
public class Message
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string? SenderUsername { get; set; } = "Server";
    public string? Title { get; set; } = "Message from Server";
    public string? Text { get; set; }
}

public class TypedMessage : Message
{
    // this just "happens" to match up with
    // Vuetify colors for snackbar
    public const string Information = "info";
    public const string Success = "success";
    public const string Warning = "warning";
    public const string Error = "error";

    public string? Type { get; set; } = Information;
}