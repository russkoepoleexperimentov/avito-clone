using MediatR;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IIdentityService _identity;

    public GetCurrentUserHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await _identity.GetUserByIdAsync(request.UserId)
            ?? throw new NotFoundException("Пользователь не найден.");

        return UserDto.FromUserInfo(user);
    }
}
