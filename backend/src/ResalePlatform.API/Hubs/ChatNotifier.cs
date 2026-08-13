using Microsoft.AspNetCore.SignalR;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Chat.Dtos;

namespace ResalePlatform.API.Hubs;

/// <summary>Реализация IChatNotifier поверх SignalR-хаба.</summary>
public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hub;

    public ChatNotifier(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public Task MessageReceivedAsync(Guid recipientUserId, MessageDto message, CancellationToken ct = default)
        => _hub.Clients.Group(recipientUserId.ToString()).SendAsync("ReceiveMessage", message, ct);
}
