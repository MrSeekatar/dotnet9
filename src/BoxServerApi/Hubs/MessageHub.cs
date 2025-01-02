using System.Security.Claims;
using BoxServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace BoxServer.Hubs;

public interface IMessageClient
{
    Task ReceiveMessage(Message message);
    Task ReceiveMessage(string username, Message message);
}

public class MessageHub(ILogger<MessageHub> logger) : Hub<IMessageClient>
{
    private readonly ILogger _logger = logger;

    public const string MessageName = "ReceiveMessage";

    public Task SendMessage(Message message)
    {
        _logger.LogInformation("Sending message with title {title}", message.Title);

        return Clients.All.ReceiveMessage(message);
    }

    public Task SendMessageToUser(string username, Message message)
    {
        _logger.LogInformation("Sending message with title {title} to user {username}", message.Title, username);

        return Clients.All.ReceiveMessage(username, message);
    }

    public override async Task OnConnectedAsync()
    {
        var group = "";
        var clientCode = Context.User?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GroupSid)?.Value;
        if (!string.IsNullOrEmpty(clientCode))
        {
            group = clientCode;
            await Groups.AddToGroupAsync(Context.ConnectionId, clientCode);
        }
        _logger.LogInformation("SignalR Client Connected with userId: '{userId}' and group: '{group}'", Context.UserIdentifier, group);
        await base.OnConnectedAsync();
    }
}