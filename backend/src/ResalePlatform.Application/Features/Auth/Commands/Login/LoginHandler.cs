using MediatR;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenService _refresh;

    public LoginHandler(
        IIdentityService identity,
        IJwtTokenGenerator jwt,
        IRefreshTokenService refresh)
    {
        _identity = identity;
        _jwt = jwt;
        _refresh = refresh;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _identity.ValidateCredentialsAsync(request.Email, request.Password)
            ?? throw new UnauthorizedException("Неверный email или пароль.");

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshToken = await _refresh.IssueAsync(user.Id, ct);

        return new AuthResponse(accessToken, expiresAt, refreshToken, UserDto.FromUserInfo(user));
    }
}
