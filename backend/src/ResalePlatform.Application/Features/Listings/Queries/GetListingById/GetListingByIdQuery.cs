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

    public GetListingByIdHandler(IApplicationDbContext db, IIdentityService identity)
    {
        _db = db;
        _identity = identity;
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
            ImageUrls = listing.Images
                .OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder)
                .Select(i => i.Url).ToList(),
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt,
        };
    }
}
