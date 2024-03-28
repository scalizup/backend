using System.Net.Mime;
using Domain.UseCases.TagGroup.Commands;
using Domain.UseCases.TagGroup.Queries;
using Domain.UseCases.Tenant;
using Domain.UseCases.Tenant.Commands;
using Domain.UseCases.Tenant.Queries;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class TagGroupController(ISender mediator) : ControllerBase
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTagGroups.TagGroupDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTagGroups.TagGroupDto>>> GetAllTagGroups(
        [FromQuery] PageQuery pageQuery,
        [FromQuery] int tenantId)
    {
        var tagGroups = await mediator.Send(new GetAllTagGroups.Query(tenantId, pageQuery));

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
    public async Task<ActionResult> DeleteTenant([FromRoute] int id)
    {
        await mediator.Send(new DeleteTagGroup.Command(id));

        return NoContent();
    }
}