using ResalePlatform.Application.Features.Chat.Dtos;

namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>Реалтайм-доставка сообщений (реализуется через SignalR в API).</summary>
public interface IChatNotifier
{
    /// <summary>Отправляет новое сообщение указанному получателю.</summary>
    Task MessageReceivedAsync(Guid recipientUserId, MessageDto message, CancellationToken ct = default);
}
