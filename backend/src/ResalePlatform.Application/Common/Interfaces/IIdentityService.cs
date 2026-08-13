using ResalePlatform.Application.Common.Models;

namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>
/// Абстракция над ASP.NET Identity. Реализуется в Infrastructure,
/// чтобы Application не зависел от UserManager напрямую.
/// </summary>
public interface IIdentityService
{
    /// <summary>Создаёт пользователя и назначает роль "User".</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors, Guid UserId)> CreateUserAsync(
        string email, string password, string displayName);

    /// <summary>Проверяет логин/пароль. Возвращает данные пользователя или null.</summary>
    Task<AppUserInfo?> ValidateCredentialsAsync(string email, string password);

    Task<AppUserInfo?> GetUserByIdAsync(Guid userId);

    /// <summary>Возвращает отображаемые имена пользователей по их id (батч).</summary>
    Task<IReadOnlyDictionary<Guid, string>> GetUserNamesAsync(IEnumerable<Guid> userIds);
}
