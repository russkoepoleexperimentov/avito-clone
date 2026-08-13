using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Listings.Dtos;

/// <summary>Элемент списка объявлений (каталог, «мои объявления»).</summary>
public class ListingListItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public decimal Price { get; init; }
    public string City { get; init; } = null!;
    public ListingCondition Condition { get; init; }
    public ListingStatus Status { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public string? PrimaryImageUrl { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>Детальная карточка объявления.</summary>
public class ListingDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public string City { get; init; } = null!;
    public ListingCondition Condition { get; init; }
    public ListingStatus Status { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public Guid UserId { get; init; }
    public string SellerName { get; init; } = null!;
    public int ViewsCount { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
