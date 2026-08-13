using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Listings.Dtos;

namespace ResalePlatform.Application.Features.Favorites.Queries.GetMyFavorites;

public record GetMyFavoritesQuery : IRequest<IReadOnlyList<ListingListItemDto>>;

public class GetMyFavoritesHandler
    : IRequestHandler<GetMyFavoritesQuery, IReadOnlyList<ListingListItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMyFavoritesHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ListingListItemDto>> Handle(
        GetMyFavoritesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        return await _db.Favorites.AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new ListingListItemDto
            {
                Id = f.Listing.Id,
                Title = f.Listing.Title,
                Price = f.Listing.Price,
                City = f.Listing.City,
                Condition = f.Listing.Condition,
                Status = f.Listing.Status,
                CategoryId = f.Listing.CategoryId,
                CategoryName = f.Listing.Category.Name,
                PrimaryImageUrl = f.Listing.Images
                    .Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
                IsFavorite = true,
                CreatedAt = f.Listing.CreatedAt,
            })
            .ToListAsync(ct);
    }
}
