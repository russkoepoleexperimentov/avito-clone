using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Features.Favorites.Commands.AddFavorite;

public record AddFavoriteCommand(Guid ListingId) : IRequest<Unit>;

public class AddFavoriteHandler : IRequestHandler<AddFavoriteCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public AddFavoriteHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        if (!await _db.Listings.AnyAsync(l => l.Id == request.ListingId, ct))
            throw new NotFoundException("Объявление не найдено.");

        // Идемпотентно: если уже в избранном — ничего не делаем.
        var exists = await _db.Favorites
            .AnyAsync(f => f.UserId == userId && f.ListingId == request.ListingId, ct);
        if (exists)
            return Unit.Value;

        _db.Favorites.Add(new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ListingId = request.ListingId,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
