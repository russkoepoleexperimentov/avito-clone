namespace ResalePlatform.Application.Common.Models;

/// <summary>
/// Лёгкое представление пользователя для Application-слоя,
/// чтобы не тащить сюда типы ASP.NET Identity из Infrastructure.
/// </summary>
public record AppUserInfo(
    Guid Id,
    string Email,
    string DisplayName,
    string? City,
    string? AvatarUrl,
    IReadOnlyList<string> Roles);
