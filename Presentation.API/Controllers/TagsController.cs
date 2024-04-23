using System.Net.Mime;
using Application.Models;
using Application.UseCases.Tags.Commands;
using Application.UseCases.Tags.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class TagsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateTag([FromBody] CreateTag.Command command)
    {
        var createdTagId = await mediator.Send(command);

        return new ObjectResult(createdTagId)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTags.TagDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTags.TagDto>>> GetAllTags(
        [FromQuery] PageQuery pageQuery,
        [FromQuery] int tagGroupId)
    {
        var tags = await mediator.Send(new GetAllTags.Query(tagGroupId, pageQuery));

        return Ok(tags);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTag([FromBody] UpdateTag.Command command)
    {
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteTag([FromRoute] int id)
    {
        await mediator.Send(new DeleteTag.Command(id));

        return NoContent();
    }
}