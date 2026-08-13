using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Listings.Commands.SetPrimaryImage;

public record SetPrimaryImageCommand(Guid ListingId, Guid ImageId) : IRequest<Unit>;

public class SetPrimaryImageHandler : IRequestHandler<SetPrimaryImageCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public SetPrimaryImageHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SetPrimaryImageCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == request.ListingId, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        if (listing.UserId != _currentUser.UserId)
            throw new ForbiddenException("Можно менять обложку только своих объявлений.");

        if (listing.Images.All(i => i.Id != request.ImageId))
            throw new NotFoundException("Фото не найдено.");

        foreach (var image in listing.Images)
            image.IsPrimary = image.Id == request.ImageId;

        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
