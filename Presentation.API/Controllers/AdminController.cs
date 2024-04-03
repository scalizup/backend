using System.Net.Mime;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class AdminController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> CreateRole([FromBody] CreateRole.Command command)
    {
        var roleCreated = await mediator.Send(command);

        return new ObjectResult(roleCreated)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    // [HttpGet]
    // public async Task<ActionResult<PageQueryResponse<GetAllTags.TagDto>>> GetAllRoles(
    //     [FromQuery] PageQuery pageQuery,
    //     [FromQuery] int tagGroupId)
    // {
    //     var tags = await mediator.Send(new GetAllTags.Query(tagGroupId, pageQuery));
    //
    //     return Ok(tags);
    // }

    [HttpPost("add-user-to-role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddUserToRole([FromBody] AddUserToRole.Command command)
    {
        await mediator.Send(command);

        return NoContent();
    }
    
    [HttpPost("add-user-to-tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddUserToTenant([FromBody] AddUserToTenant.Command command)
    {
        await mediator.Send(command);

        return NoContent();
    }
}