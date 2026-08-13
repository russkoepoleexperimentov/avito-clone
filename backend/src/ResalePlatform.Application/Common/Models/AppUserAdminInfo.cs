namespace ResalePlatform.Application.Common.Models;

/// <summary>Данные пользователя для админ-панели.</summary>
public record AppUserAdminInfo(
    Guid Id,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles,
    bool IsBlocked,
    DateTimeOffset CreatedAt);
