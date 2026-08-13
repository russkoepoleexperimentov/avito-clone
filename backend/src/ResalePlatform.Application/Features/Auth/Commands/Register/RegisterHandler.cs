using MediatR;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenService _refresh;

    public RegisterHandler(
        IIdentityService identity,
        IJwtTokenGenerator jwt,
        IRefreshTokenService refresh)
    {
        _identity = identity;
        _jwt = jwt;
        _refresh = refresh;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        var (succeeded, errors, userId) = await _identity.CreateUserAsync(
            request.Email, request.Password, request.DisplayName);

        if (!succeeded)
            throw new ConflictException(string.Join("; ", errors));

        var user = await _identity.GetUserByIdAsync(userId)
            ?? throw new ConflictException("Не удалось создать пользователя.");

        var (accessToken, expiresAt) = _jwt.GenerateAccessToken(user);
        var refreshToken = await _refresh.IssueAsync(user.Id, ct);

        return new AuthResponse(accessToken, expiresAt, refreshToken, UserDto.FromUserInfo(user));
    }
}
