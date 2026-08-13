using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Features.Auth.Commands.Login;
using ResalePlatform.Application.Features.Auth.Commands.Refresh;
using ResalePlatform.Application.Features.Auth.Commands.Register;
using ResalePlatform.Application.Features.Auth.Dtos;
using ResalePlatform.Application.Features.Auth.Queries.GetCurrentUser;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshCommand command)
        => Ok(await _mediator.Send(command));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId))
            throw new UnauthorizedException("Некорректный токен.");

        return Ok(await _mediator.Send(new GetCurrentUserQuery(userId)));
    }
}
