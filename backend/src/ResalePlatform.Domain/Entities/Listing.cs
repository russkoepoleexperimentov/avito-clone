using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Domain.Entities;

/// <summary>
/// Объявление о продаже — центральная сущность платформы.
/// </summary>
public class Listing
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public ListingCondition Condition { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Draft;

    public string City { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    /// <summary>Владелец объявления (Id пользователя из Identity).</summary>
    public Guid UserId { get; set; }

    public int ViewsCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
}
