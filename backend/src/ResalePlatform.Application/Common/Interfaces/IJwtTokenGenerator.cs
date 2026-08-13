using ResalePlatform.Application.Common.Models;

namespace ResalePlatform.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    /// <summary>Генерирует подписанный access-токен (JWT) для пользователя.</summary>
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(AppUserInfo user);
}
