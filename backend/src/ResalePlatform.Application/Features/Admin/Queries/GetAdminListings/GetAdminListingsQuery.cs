using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Common.Models;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Admin.Queries.GetAdminListings;

public class AdminListingDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public decimal Price { get; init; }
    public string City { get; init; } = null!;
    public ListingStatus Status { get; init; }
    public Guid UserId { get; init; }
    public string SellerName { get; set; } = "—";
    public DateTimeOffset CreatedAt { get; init; }
}

public record GetAdminListingsQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<AdminListingDto>>;

public class GetAdminListingsHandler
    : IRequestHandler<GetAdminListingsQuery, PagedResult<AdminListingDto>>
{
    private const int MaxPageSize = 100;

    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;

    public GetAdminListingsHandler(IApplicationDbContext db, IIdentityService identity)
    {
        _db = db;
        _identity = identity;
    }

    public async Task<PagedResult<AdminListingDto>> Handle(
        GetAdminListingsQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var query = _db.Listings.AsNoTracking().OrderByDescending(l => l.CreatedAt);
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AdminListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price,
                City = l.City,
                Status = l.Status,
                UserId = l.UserId,
                CreatedAt = l.CreatedAt,
            })
            .ToListAsync(ct);

        var names = await _identity.GetUserNamesAsync(items.Select(i => i.UserId));
        foreach (var item in items)
            item.SellerName = names.GetValueOrDefault(item.UserId, "—");

        return new PagedResult<AdminListingDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }
}
