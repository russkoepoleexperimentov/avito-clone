using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string Slug, Guid? ParentId, int SortOrder)
    : IRequest<Unit>;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public UpdateCategoryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException("Категория не найдена.");

        if (request.ParentId == request.Id)
            throw new ConflictException("Категория не может быть родителем самой себя.");

        if (await _db.Categories.AnyAsync(c => c.Slug == request.Slug && c.Id != request.Id, ct))
            throw new ConflictException($"Категория со slug '{request.Slug}' уже существует.");

        if (request.ParentId is { } parentId &&
            !await _db.Categories.AnyAsync(c => c.Id == parentId, ct))
            throw new NotFoundException("Родительская категория не найдена.");

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;

        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
