using MediatR;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.Application.Features.Admin.Commands.SetUserBlocked;

public record SetUserBlockedCommand(Guid UserId, bool Blocked) : IRequest<Unit>;

public class SetUserBlockedHandler : IRequestHandler<SetUserBlockedCommand, Unit>
{
    private readonly IIdentityService _identity;

    public SetUserBlockedHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<Unit> Handle(SetUserBlockedCommand request, CancellationToken ct)
    {
        var ok = await _identity.SetBlockedAsync(request.UserId, request.Blocked);
        if (!ok)
            throw new ConflictException("Пользователь не найден или является администратором.");

        return Unit.Value;
    }
}
