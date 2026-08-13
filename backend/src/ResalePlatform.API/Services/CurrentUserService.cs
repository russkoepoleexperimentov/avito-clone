using System.Security.Claims;
using ResalePlatform.Application.Common.Interfaces;

namespace ResalePlatform.API.Services;

/// <summary>Достаёт данные текущего пользователя из JWT-претензий HttpContext.</summary>
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool IsAdmin => _accessor.HttpContext?.User.IsInRole("Admin") ?? false;
}
