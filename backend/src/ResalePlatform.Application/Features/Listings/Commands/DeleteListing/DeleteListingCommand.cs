using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Listings.Commands.DeleteListing;

public record DeleteListingCommand(Guid Id) : IRequest<Unit>;

public class DeleteListingHandler : IRequestHandler<DeleteListingCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;

    public DeleteListingHandler(IApplicationDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteListingCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.Id, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        // Удалять может владелец или администратор.
        if (listing.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException("Можно удалять только свои объявления.");

        _db.Listings.Remove(listing);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
