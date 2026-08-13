using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Listings.Dtos;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Listings.Queries.GetMyListings;

/// <summary>Объявления текущего пользователя, опционально отфильтрованные по статусу.</summary>
public record GetMyListingsQuery(ListingStatus? Status) : IRequest<IReadOnlyList<ListingListItemDto>>;

public class GetMyListingsHandler
    : IRequestHandler<GetMyListingsQuery, IReadOnlyList<ListingListItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetMyListingsHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ListingListItemDto>> Handle(
        GetMyListingsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        var query = _db.Listings.AsNoTracking().Where(l => l.UserId == userId);

        if (request.Status is { } status)
            query = query.Where(l => l.Status == status);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new ListingListItemDto
            {
                Id = l.Id,
                Title = l.Title,
                Price = l.Price,
                City = l.City,
                Condition = l.Condition,
                Status = l.Status,
                CategoryId = l.CategoryId,
                CategoryName = l.Category.Name,
                PrimaryImageUrl = l.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                CreatedAt = l.CreatedAt,
            })
            .ToListAsync(ct);
    }
}
