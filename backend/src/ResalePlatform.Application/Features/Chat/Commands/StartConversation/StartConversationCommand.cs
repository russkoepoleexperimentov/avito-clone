using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Features.Chat.Commands.StartConversation;

/// <summary>Открывает (или находит существующий) диалог с продавцом по объявлению.</summary>
public record StartConversationCommand(Guid ListingId) : IRequest<Guid>;

public class StartConversationHandler : IRequestHandler<StartConversationCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public StartConversationHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(StartConversationCommand request, CancellationToken ct)
    {
        var buyerId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        if (listing.UserId == buyerId)
            throw new ConflictException("Нельзя написать самому себе.");

        var existing = await _db.Conversations.FirstOrDefaultAsync(
            c => c.ListingId == request.ListingId && c.BuyerId == buyerId, ct);
        if (existing is not null)
            return existing.Id;

        var now = DateTimeOffset.UtcNow;
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ListingId = listing.Id,
            BuyerId = buyerId,
            SellerId = listing.UserId,
            CreatedAt = now,
            LastMessageAt = now,
        };
        _db.Conversations.Add(conversation);
        await _db.SaveChangesAsync(ct);

        return conversation.Id;
    }
}
