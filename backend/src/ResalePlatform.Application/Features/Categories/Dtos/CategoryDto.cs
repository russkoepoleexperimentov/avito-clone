namespace ResalePlatform.Application.Features.Categories.Dtos;

public class CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; }
    public List<CategoryDto> Children { get; init; } = new();
}
