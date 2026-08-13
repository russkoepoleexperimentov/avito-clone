using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Common.Models;
using ResalePlatform.Application.Features.Admin.Commands.SetUserBlocked;
using ResalePlatform.Application.Features.Admin.Queries.GetAdminListings;
using ResalePlatform.Application.Features.Admin.Queries.GetUsers;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<AppUserAdminInfo>>> GetUsers()
        => Ok(await _mediator.Send(new GetUsersQuery()));

    [HttpPut("users/{id:guid}/blocked")]
    public async Task<IActionResult> SetBlocked(Guid id, [FromBody] SetBlockedBody body)
    {
        await _mediator.Send(new SetUserBlockedCommand(id, body.Blocked));
        return NoContent();
    }

    [HttpGet("listings")]
    public async Task<ActionResult<PagedResult<AdminListingDto>>> GetListings(
        [FromQuery] GetAdminListingsQuery query)
        => Ok(await _mediator.Send(query));

    public record SetBlockedBody(bool Blocked);
}
