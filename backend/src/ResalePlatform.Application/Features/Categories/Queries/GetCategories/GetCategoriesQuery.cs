using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Categories.Dtos;

namespace ResalePlatform.Application.Features.Categories.Queries.GetCategories;

/// <summary>Возвращает все категории в виде дерева (корни с вложенными детьми).</summary>
public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCategoriesHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var all = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ParentId = c.ParentId,
                SortOrder = c.SortOrder,
            })
            .ToListAsync(ct);

        // Собираем дерево в памяти.
        var byParent = all.ToLookup(c => c.ParentId);
        foreach (var node in all)
            node.Children.AddRange(byParent[node.Id]);

        return byParent[null].ToList();
    }
}
