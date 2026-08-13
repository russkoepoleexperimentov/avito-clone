using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Chat.Dtos;

namespace ResalePlatform.Application.Features.Chat.Queries.GetMessages;

/// <summary>История сообщений диалога. Помечает входящие как прочитанные.</summary>
public record GetMessagesQuery(Guid ConversationId) : IRequest<IReadOnlyList<MessageDto>>;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMessagesHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<MessageDto>> Handle(GetMessagesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId, ct)
            ?? throw new NotFoundException("Диалог не найден.");

        if (conversation.BuyerId != userId && conversation.SellerId != userId)
            throw new ForbiddenException("Вы не участник этого диалога.");

        // Отмечаем входящие сообщения прочитанными.
        await _db.Messages
            .Where(m => m.ConversationId == request.ConversationId
                        && m.SenderId != userId && !m.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true), ct);

        return await _db.Messages.AsNoTracking()
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                Text = m.Text,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt,
            })
            .ToListAsync(ct);
    }
}
