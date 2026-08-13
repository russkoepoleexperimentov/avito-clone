namespace ResalePlatform.Application.Common.Interfaces;

/// <summary>Данные текущего аутентифицированного пользователя (из JWT).</summary>
public interface ICurrentUser
{
    /// <summary>Id пользователя или null, если запрос анонимный.</summary>
    Guid? UserId { get; }

    bool IsAdmin { get; }
}
