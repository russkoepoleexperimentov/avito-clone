namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>Загружаемый файл, не зависящий от ASP.NET (IFormFile остаётся в API).</summary>
public record UploadFile(Stream Content, string FileName, string ContentType, long Length);

/// <summary>Абстракция хранилища файлов (локальный диск / S3 / ...).</summary>
public interface IFileStorage
{
    /// <summary>Сохраняет файл и возвращает относительный URL (напр. /uploads/xxx.jpg).</summary>
    Task<string> SaveAsync(UploadFile file, CancellationToken ct = default);

    /// <summary>Удаляет файл по ранее возвращённому URL.</summary>
    Task DeleteAsync(string url, CancellationToken ct = default);
}
