namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>
/// Управление refresh-токенами (хранятся в БД, ротация при обновлении).
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>Создаёт новый refresh-токен для пользователя и сохраняет его.</summary>
    Task<string> IssueAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Проверяет refresh-токен и одноразово гасит его (ротация).
    /// Возвращает Id пользователя, либо null если токен невалиден/просрочен/отозван.
    /// </summary>
    Task<Guid?> ConsumeAsync(string token, CancellationToken ct = default);

    /// <summary>Отзывает все активные refresh-токены пользователя (logout).</summary>
    Task RevokeAllAsync(Guid userId, CancellationToken ct = default);
}
