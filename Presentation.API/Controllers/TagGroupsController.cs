using System.Net.Mime;
using Application.Models;
using Application.UseCases.TagGroups.Commands;
using Application.UseCases.TagGroups.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class TagGroupsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateTagGroup([FromBody] CreateTagGroup.Command command)
    {
        var createdTagGroupId = await mediator.Send(command);

        return new ObjectResult(createdTagGroupId)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTagGroupById.TagGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetTagGroupById.TagGroupDto>> GetTagGroupById([FromRoute] int id)
    {
        var tagGroup = await mediator.Send(new GetTagGroupById.Query(id));

        return Ok(tagGroup);
    }
    
    [HttpGet("search/{searchTerm:alpha}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTagGroupById.TagGroupDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetTagGroupById.TagGroupDto>> GetTagGroupWithTagsBySearchTerm([FromRoute] string searchTerm)
    {
        var tagGroup = await mediator.Send(new GetTagGroupWithTagsBySearchTerm.Query(searchTerm));

        return Ok(tagGroup);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTagGroups.TagGroupDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTagGroups.TagGroupDto>>> GetAllTagGroups(
        [FromQuery] PageQuery pageQuery,
        [FromQuery] int tenantId)
    {
        var tagGroups = await mediator.Send(new GetAllTagGroups.Query(pageQuery)
        {
            TenantId = tenantId
        });

        return Ok(tagGroups);
    }
    
    [HttpGet("tags")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTagGroups.TagGroupDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTagGroups.TagGroupDto>>> GetAllTagGroupsWithTags(
        [FromQuery] PageQuery pageQuery)
    {
        var tagGroups = await mediator.Send(new GetAllTagGroupsWithTags.Query(pageQuery));

        return Ok(tagGroups);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTagGroup([FromBody] UpdateTagGroup.Command command)
    {
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteTagGroup([FromRoute] int id)
    {
        await mediator.Send(new DeleteTagGroup.Command(id));

        return NoContent();
    }
}