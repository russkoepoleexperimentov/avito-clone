namespace ResalePlatform.Domain.Entities;

/// <summary>Избранное: связь пользователь ↔ объявление.</summary>
public class Favorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}
