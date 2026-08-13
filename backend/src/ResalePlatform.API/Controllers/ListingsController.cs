using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Listings.Commands.AddListingImages;
using ResalePlatform.Application.Features.Listings.Commands.CreateListing;
using ResalePlatform.Application.Features.Listings.Commands.DeleteListing;
using ResalePlatform.Application.Features.Listings.Commands.DeleteListingImage;
using ResalePlatform.Application.Features.Listings.Commands.SetPrimaryImage;
using ResalePlatform.Application.Features.Listings.Commands.UpdateListing;
using ResalePlatform.Application.Features.Listings.Dtos;
using ResalePlatform.Application.Features.Listings.Queries.GetListingById;
using ResalePlatform.Application.Features.Listings.Queries.GetMyListings;
using ResalePlatform.Domain.Enums;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ListingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Карточка объявления (публично).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ListingDto>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetListingByIdQuery(id)));

    /// <summary>Объявления текущего пользователя.</summary>
    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<ListingListItemDto>>> GetMine(
        [FromQuery] ListingStatus? status)
        => Ok(await _mediator.Send(new GetMyListingsQuery(status)));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(CreateListingCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateListingCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id в пути и теле не совпадают.");

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteListingCommand(id));
        return NoContent();
    }

    /// <summary>Загрузка фотографий к объявлению (multipart/form-data, поле "files").</summary>
    [Authorize]
    [HttpPost("{id:guid}/images")]
    public async Task<ActionResult<IReadOnlyList<ListingImageDto>>> UploadImages(
        Guid id, [FromForm] List<IFormFile> files)
    {
        var uploads = files.Select(f =>
            new UploadFile(f.OpenReadStream(), f.FileName, f.ContentType, f.Length)).ToList();

        var result = await _mediator.Send(new AddListingImagesCommand(id, uploads));
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
    {
        await _mediator.Send(new DeleteListingImageCommand(id, imageId));
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/images/{imageId:guid}/primary")]
    public async Task<IActionResult> SetPrimaryImage(Guid id, Guid imageId)
    {
        await _mediator.Send(new SetPrimaryImageCommand(id, imageId));
        return NoContent();
    }
}
