using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Listings.Dtos;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Features.Listings.Commands.AddListingImages;

public record AddListingImagesCommand(Guid ListingId, IReadOnlyList<UploadFile> Files)
    : IRequest<IReadOnlyList<ListingImageDto>>;

public class AddListingImagesHandler
    : IRequestHandler<AddListingImagesCommand, IReadOnlyList<ListingImageDto>>
{
    public const int MaxImagesPerListing = 10;
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 МБ
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IFileStorage _storage;

    public AddListingImagesHandler(
        IApplicationDbContext db, ICurrentUser currentUser, IFileStorage storage)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<IReadOnlyList<ListingImageDto>> Handle(
        AddListingImagesCommand request, CancellationToken ct)
    {
        if (request.Files.Count == 0)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["files"] = new[] { "Не переданы файлы." },
            });

        var listing = await _db.Listings
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == request.ListingId, ct)
            ?? throw new NotFoundException("Объявление не найдено.");

        if (listing.UserId != _currentUser.UserId)
            throw new ForbiddenException("Можно добавлять фото только к своим объявлениям.");

        if (listing.Images.Count + request.Files.Count > MaxImagesPerListing)
            throw new ConflictException($"Не более {MaxImagesPerListing} фото на объявление.");

        foreach (var file in request.Files)
            ValidateFile(file);

        var startOrder = listing.Images.Count == 0 ? 0 : listing.Images.Max(i => i.SortOrder) + 1;
        var hasPrimary = listing.Images.Any(i => i.IsPrimary);

        var added = new List<ListingImageDto>();
        var order = startOrder;

        foreach (var file in request.Files)
        {
            var url = await _storage.SaveAsync(file, ct);
            var image = new ListingImage
            {
                Id = Guid.NewGuid(),
                ListingId = listing.Id,
                Url = url,
                IsPrimary = !hasPrimary && order == startOrder, // первое фото — обложка
                SortOrder = order++,
            };
            _db.ListingImages.Add(image);
            added.Add(new ListingImageDto
            {
                Id = image.Id,
                Url = image.Url,
                IsPrimary = image.IsPrimary,
                SortOrder = image.SortOrder,
            });
        }

        await _db.SaveChangesAsync(ct);
        return added;
    }

    private static void ValidateFile(UploadFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["files"] = new[] { $"Недопустимый формат '{ext}'. Разрешены: jpg, png, webp." },
            });

        if (file.Length <= 0 || file.Length > MaxFileSizeBytes)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["files"] = new[] { "Размер файла должен быть от 1 байта до 5 МБ." },
            });
    }
}
