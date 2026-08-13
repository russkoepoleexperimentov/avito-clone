using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Categories.Dtos;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Slug, Guid? ParentId, int SortOrder)
    : IRequest<CategoryDto>;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _db;

    public CreateCategoryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (await _db.Categories.AnyAsync(c => c.Slug == request.Slug, ct))
            throw new ConflictException($"Категория со slug '{request.Slug}' уже существует.");

        if (request.ParentId is { } parentId &&
            !await _db.Categories.AnyAsync(c => c.Id == parentId, ct))
            throw new NotFoundException("Родительская категория не найдена.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder,
        };
    }
}
