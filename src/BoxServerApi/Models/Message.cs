namespace BoxServer.Models;

public class Message
{
    // this just "happen" to match up with
    // Vuetify colors for snackbar
    public const string Information = "info";
    public const string Success = "success";
    public const string Warning = "warning";
    public const string Error = "error";

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string? SenderUsername { get; set; } = "Server";
    public string? Title { get; set; } = "Message from Server";
    public string? Text { get; set; }
    public string? Type { get; set; } = Information;
}