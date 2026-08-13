using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Listings.Commands.UpdateListing;

public record UpdateListingCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    ListingCondition Condition,
    ListingStatus Status,
    string City,
    Guid CategoryId) : IRequest<Unit>;

public class UpdateListingHandler : IRequestHandler<UpdateListingCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public UpdateListingHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateListingCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.Id, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        // Редактировать может только владелец.
        if (listing.UserId != _currentUser.UserId)
            throw new ForbiddenException("Можно редактировать только свои объявления.");

        if (request.CategoryId != listing.CategoryId &&
            !await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException("Категория не найдена.");

        listing.Title = request.Title;
        listing.Description = request.Description;
        listing.Price = request.Price;
        listing.Condition = request.Condition;
        listing.Status = request.Status;
        listing.City = request.City;
        listing.CategoryId = request.CategoryId;
        listing.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
