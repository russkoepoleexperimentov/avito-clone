using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.API.Services;

/// <summary>
/// Хранит файлы в wwwroot/uploads и отдаёт относительный URL /uploads/{файл}.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private const string UploadsFolder = "uploads";
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    private string WebRoot => _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

    public async Task<string> SaveAsync(UploadFile file, CancellationToken ct = default)
    {
        var uploadsPath = Path.Combine(WebRoot, UploadsFolder);
        Directory.CreateDirectory(uploadsPath);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.Content.CopyToAsync(stream, ct);
        }

        return $"/{UploadsFolder}/{fileName}";
    }

    public Task DeleteAsync(string url, CancellationToken ct = default)
    {
        // url вида /uploads/xxx.jpg -> физический путь в wwwroot.
        var relative = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(WebRoot, relative);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
