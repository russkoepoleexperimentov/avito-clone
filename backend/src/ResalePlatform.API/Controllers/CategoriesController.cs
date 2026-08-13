using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Features.Categories.Commands.CreateCategory;
using ResalePlatform.Application.Features.Categories.Commands.DeleteCategory;
using ResalePlatform.Application.Features.Categories.Commands.UpdateCategory;
using ResalePlatform.Application.Features.Categories.Dtos;
using ResalePlatform.Application.Features.Categories.Queries.GetCategories;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Дерево категорий (публично).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Get()
        => Ok(await _mediator.Send(new GetCategoriesQuery()));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryCommand command)
    {
        var created = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id в пути и теле не совпадают.");

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
