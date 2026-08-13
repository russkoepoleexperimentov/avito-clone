using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public DeleteCategoryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException("Категория не найдена.");

        if (await _db.Categories.AnyAsync(c => c.ParentId == request.Id, ct))
            throw new ConflictException("Нельзя удалить категорию с подкатегориями.");

        if (await _db.Listings.AnyAsync(l => l.CategoryId == request.Id, ct))
            throw new ConflictException("Нельзя удалить категорию, в которой есть объявления.");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
