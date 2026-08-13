using MediatR;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Refresh;

public class RefreshHandler : IRequestHandler<RefreshCommand, AuthResponse>
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenService _refresh;

    public RefreshHandler(
        IIdentityService identity,
        IJwtTokenGenerator jwt,
        IRefreshTokenService refresh)
    {
        _identity = identity;
        _jwt = jwt;
        _refresh = refresh;
    }

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken ct)
    {
        // Проверяем и одноразово гасим переданный refresh-токен (ротация).
        var userId = await _refresh.ConsumeAsync(request.RefreshToken, ct)
            ?? throw new UnauthorizedException("Refresh-токен недействителен или истёк.");

        var user = await _identity.GetUserByIdAsync(userId)
            ?? throw new UnauthorizedException("Пользователь не найден.");

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshToken = await _refresh.IssueAsync(user.Id, ct);

        return new AuthResponse(accessToken, expiresAt, refreshToken, UserDto.FromUserInfo(user));
    }
}
