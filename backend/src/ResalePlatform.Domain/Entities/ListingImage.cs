namespace ResalePlatform.Domain.Entities;

/// <summary>Фотография объявления.</summary>
public class ListingImage
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public string Url { get; set; } = null!;

    /// <summary>Обложка объявления (одно фото на объявление).</summary>
    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}
