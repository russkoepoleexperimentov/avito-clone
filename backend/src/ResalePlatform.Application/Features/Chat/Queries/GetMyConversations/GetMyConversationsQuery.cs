using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Chat.Dtos;

namespace ResalePlatform.Application.Features.Chat.Queries.GetMyConversations;

public record GetMyConversationsQuery : IRequest<IReadOnlyList<ConversationDto>>;

public class GetMyConversationsHandler
    : IRequestHandler<GetMyConversationsQuery, IReadOnlyList<ConversationDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identity;

    public GetMyConversationsHandler(
        IApplicationDbContext db, ICurrentUser currentUser, IIdentityService identity)
    {
        _db = db;
        _currentUser = currentUser;
        _identity = identity;
    }

    public async Task<IReadOnlyList<ConversationDto>> Handle(
        GetMyConversationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        var conversations = await _db.Conversations.AsNoTracking()
            .Where(c => c.BuyerId == userId || c.SellerId == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new ConversationDto
            {
                Id = c.Id,
                ListingId = c.ListingId,
                ListingTitle = c.Listing.Title,
                ListingImageUrl = c.Listing.Images
                    .Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
                OtherUserId = c.BuyerId == userId ? c.SellerId : c.BuyerId,
                LastMessageText = _db.Messages
                    .Where(m => m.ConversationId == c.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Text)
                    .FirstOrDefault(),
                LastMessageAt = c.LastMessageAt,
            })
            .ToListAsync(ct);

        // Подставляем имена собеседников (батч).
        var names = await _identity.GetUserNamesAsync(conversations.Select(c => c.OtherUserId));
        foreach (var conv in conversations)
            conv.OtherUserName = names.GetValueOrDefault(conv.OtherUserId, "—");

        return conversations;
    }
}
