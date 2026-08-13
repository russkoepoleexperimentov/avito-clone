using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ResalePlatform.API.Hubs;

/// <summary>
/// Хаб чата. Каждое соединение добавляется в группу с именем = id пользователя,
/// чтобы адресно доставлять входящие сообщения на все его устройства/вкладки.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        await base.OnConnectedAsync();
    }
}
