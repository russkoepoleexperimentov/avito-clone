namespace ResalePlatform.Domain.Entities;

/// <summary>
/// Иерархический справочник категорий (родитель -> подкатегории).
/// </summary>
public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>Человекочитаемый идентификатор для URL, напр. "electronics".</summary>
    public string Slug { get; set; } = null!;

    /// <summary>null => корневая категория.</summary>
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }

    public int SortOrder { get; set; }

    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
