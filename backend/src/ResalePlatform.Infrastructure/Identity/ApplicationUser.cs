using Microsoft.AspNetCore.Identity;

namespace ResalePlatform.Infrastructure.Identity;

/// <summary>
/// Пользователь платформы. Расширяет стандартного пользователя ASP.NET Identity.
/// Живёт в Infrastructure, т.к. зависит от Identity; Domain остаётся чистым.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = null!;
    public string? City { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsBlocked { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
