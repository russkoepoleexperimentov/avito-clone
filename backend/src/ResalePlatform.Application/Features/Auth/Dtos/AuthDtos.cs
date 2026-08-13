using ResalePlatform.Application.Common.Models;

namespace ResalePlatform.Application.Features.Auth.Dtos;

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? City,
    string? AvatarUrl,
    IReadOnlyList<string> Roles)
{
    public static UserDto FromUserInfo(AppUserInfo u) =>
        new(u.Id, u.Email, u.DisplayName, u.City, u.AvatarUrl, u.Roles);
}

public record AuthResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    UserDto User);
