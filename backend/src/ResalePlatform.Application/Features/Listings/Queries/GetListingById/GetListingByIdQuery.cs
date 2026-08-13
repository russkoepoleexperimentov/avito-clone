using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Listings.Dtos;

namespace ResalePlatform.Application.Features.Listings.Queries.GetListingById;

public record GetListingByIdQuery(Guid Id) : IRequest<ListingDto>;

public class GetListingByIdHandler : IRequestHandler<GetListingByIdQuery, ListingDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;
    private readonly ICurrentUser _currentUser;

    public GetListingByIdHandler(
        IApplicationDbContext db, IIdentityService identity, ICurrentUser currentUser)
    {
        _db = db;
        _identity = identity;
        _currentUser = currentUser;
    }

    public async Task<ListingDto> Handle(GetListingByIdQuery request, CancellationToken ct)
    {
        var listing = await _db.Listings
            .Include(l => l.Category)
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == request.Id, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        // Счётчик просмотров (простой инкремент — для MVP достаточно).
        listing.ViewsCount++;
        await _db.SaveChangesAsync(ct);

        var seller = await _identity.GetUserByIdAsync(listing.UserId);

        var userId = _currentUser.UserId;
        var isFavorite = userId != null
            && await _db.Favorites.AnyAsync(
                f => f.UserId == userId && f.ListingId == listing.Id, ct);

        return new ListingDto
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            City = listing.City,
            Condition = listing.Condition,
            Status = listing.Status,
            CategoryId = listing.CategoryId,
            CategoryName = listing.Category.Name,
            UserId = listing.UserId,
            SellerName = seller?.DisplayName ?? "—",
            ViewsCount = listing.ViewsCount,
            IsFavorite = isFavorite,
            Images = listing.Images
                .OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder)
                .Select(i => new ListingImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder,
                })
                .ToList(),
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt,
        };
    }
}
