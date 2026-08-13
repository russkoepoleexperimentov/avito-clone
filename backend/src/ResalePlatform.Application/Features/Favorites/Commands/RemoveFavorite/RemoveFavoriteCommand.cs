using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Favorites.Commands.RemoveFavorite;

public record RemoveFavoriteCommand(Guid ListingId) : IRequest<Unit>;

public class RemoveFavoriteHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public RemoveFavoriteHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        // Идемпотентно: удаляем, если есть; иначе просто ок.
        await _db.Favorites
            .Where(f => f.UserId == userId && f.ListingId == request.ListingId)
            .ExecuteDeleteAsync(ct);

        return Unit.Value;
    }
}
