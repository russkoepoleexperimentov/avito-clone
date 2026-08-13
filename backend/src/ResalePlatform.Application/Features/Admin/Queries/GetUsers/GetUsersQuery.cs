using MediatR;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Common.Models;

namespace ResalePlatform.Application.Features.Admin.Queries.GetUsers;

public record GetUsersQuery : IRequest<IReadOnlyList<AppUserAdminInfo>>;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<AppUserAdminInfo>>
{
    private readonly IIdentityService _identity;

    public GetUsersHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public Task<IReadOnlyList<AppUserAdminInfo>> Handle(GetUsersQuery request, CancellationToken ct)
        => _identity.GetAllUsersAsync();
}
