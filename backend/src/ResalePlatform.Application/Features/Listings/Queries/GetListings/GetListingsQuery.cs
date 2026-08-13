using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Common.Models;
using ResalePlatform.Application.Features.Listings.Dtos;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Listings.Queries.GetListings;

/// <summary>Каталог активных объявлений с поиском, фильтрами, сортировкой и пагинацией.</summary>
public record GetListingsQuery(
    string? Search,
    string? CategorySlug,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? City,
    string? Sort,
    int Page = 1,
    int PageSize = 12) : IRequest<PagedResult<ListingListItemDto>>;

public class GetListingsHandler
    : IRequestHandler<GetListingsQuery, PagedResult<ListingListItemDto>>
{
    private const int MaxPageSize = 48;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public GetListingsHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<ListingListItemDto>> Handle(
        GetListingsQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var query = _db.Listings.AsNoTracking()
            .Where(l => l.Status == ListingStatus.Active);

        // Фильтр по категории: сама категория + все её потомки.
        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            var categoryIds = await GetCategoryWithDescendantsAsync(request.CategorySlug, ct);
            query = query.Where(l => categoryIds.Contains(l.CategoryId));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(l =>
                l.Title.ToLower().Contains(term) || l.Description.ToLower().Contains(term));
        }

        if (request.MinPrice is { } min)
            query = query.Where(l => l.Price >= min);
        if (request.MaxPrice is { } max)
            query = query.Where(l => l.Price <= max);

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();
            query = query.Where(l => l.City.ToLower().Contains(city));
        }

        query = request.Sort switch
        {
            "price_asc" => query.OrderBy(l => l.Price),
            "price_desc" => query.OrderByDescending(l => l.Price),
            _ => query.OrderByDescending(l => l.CreatedAt),
        };

        var totalCount = await query.CountAsync(ct);

        var userId = _currentUser.UserId;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
                    .Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
                IsFavorite = userId != null
                    && _db.Favorites.Any(f => f.UserId == userId && f.ListingId == l.Id),
                CreatedAt = l.CreatedAt,
            })
            .ToListAsync(ct);

        return new PagedResult<ListingListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    /// <summary>Возвращает id категории по slug плюс id всех её потомков.</summary>
    private async Task<HashSet<Guid>> GetCategoryWithDescendantsAsync(
        string slug, CancellationToken ct)
    {
        var all = await _db.Categories.AsNoTracking()
            .Select(c => new { c.Id, c.Slug, c.ParentId })
            .ToListAsync(ct);

        var root = all.FirstOrDefault(c => c.Slug == slug);
        if (root is null)
            return new HashSet<Guid>();

        var byParent = all.ToLookup(c => c.ParentId);
        var result = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(root.Id);

        while (stack.Count > 0)
        {
            var id = stack.Pop();
            if (!result.Add(id))
                continue;
            foreach (var child in byParent[id])
                stack.Push(child.Id);
        }

        return result;
    }
}
