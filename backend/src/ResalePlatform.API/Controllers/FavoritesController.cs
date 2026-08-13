using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Features.Favorites.Commands.AddFavorite;
using ResalePlatform.Application.Features.Favorites.Commands.RemoveFavorite;
using ResalePlatform.Application.Features.Favorites.Queries.GetMyFavorites;
using ResalePlatform.Application.Features.Listings.Dtos;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Избранные объявления текущего пользователя.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ListingListItemDto>>> Get()
        => Ok(await _mediator.Send(new GetMyFavoritesQuery()));

    [HttpPost("{listingId:guid}")]
    public async Task<IActionResult> Add(Guid listingId)
    {
        await _mediator.Send(new AddFavoriteCommand(listingId));
        return NoContent();
    }

    [HttpDelete("{listingId:guid}")]
    public async Task<IActionResult> Remove(Guid listingId)
    {
        await _mediator.Send(new RemoveFavoriteCommand(listingId));
        return NoContent();
    }
}
