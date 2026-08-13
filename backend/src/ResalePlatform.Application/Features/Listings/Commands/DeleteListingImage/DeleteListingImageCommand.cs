using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Listings.Commands.DeleteListingImage;

public record DeleteListingImageCommand(Guid ListingId, Guid ImageId) : IRequest<Unit>;

public class DeleteListingImageHandler : IRequestHandler<DeleteListingImageCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IFileStorage _storage;

    public DeleteListingImageHandler(
        IApplicationDbContext db, ICurrentUser currentUser, IFileStorage storage)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<Unit> Handle(DeleteListingImageCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == request.ListingId, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        if (listing.UserId != _currentUser.UserId)
            throw new ForbiddenException("Можно удалять фото только своих объявлений.");

        var image = listing.Images.FirstOrDefault(i => i.Id == request.ImageId)
            ?? throw new NotFoundException("Фото не найдено.");

        _db.ListingImages.Remove(image);
        await _storage.DeleteAsync(image.Url, ct);

        // Если удалили обложку — назначаем новую (первую по порядку).
        if (image.IsPrimary)
        {
            var next = listing.Images
                .Where(i => i.Id != image.Id)
                .OrderBy(i => i.SortOrder)
                .FirstOrDefault();
            if (next is not null)
                next.IsPrimary = true;
        }

        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
