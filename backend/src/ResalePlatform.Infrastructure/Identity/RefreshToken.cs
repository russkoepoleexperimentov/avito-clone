namespace ResalePlatform.Infrastructure.Identity;

/// <summary>
/// Refresh-токен, хранится в БД. Одноразовый: при обновлении помечается отозванным.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    /// <summary>Случайное непредсказуемое значение токена.</summary>
    public string Token { get; set; } = null!;

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }

    public bool IsActive => RevokedAt is null && DateTimeOffset.UtcNow < ExpiresAt;
}
