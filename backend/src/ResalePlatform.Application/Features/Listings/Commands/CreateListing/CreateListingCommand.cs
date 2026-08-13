using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Domain.Entities;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.Application.Features.Listings.Commands.CreateListing;

public record CreateListingCommand(
    string Title,
    string Description,
    decimal Price,
    ListingCondition Condition,
    string City,
    Guid CategoryId) : IRequest<Guid>;

public class CreateListingHandler : IRequestHandler<CreateListingCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public CreateListingHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateListingCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        if (!await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException("Категория не найдена.");

        var now = DateTimeOffset.UtcNow;
        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Condition = request.Condition,
            Status = ListingStatus.Active,
            City = request.City,
            CategoryId = request.CategoryId,
            UserId = userId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _db.Listings.Add(listing);
        await _db.SaveChangesAsync(ct);

        return listing.Id;
    }
}
