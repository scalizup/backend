using System.Net.Mime;
using Application.UseCases.Auth.Roles.Commands;
using Application.UseCases.Auth.Roles.Queries;
using Application.UseCases.Auth.Tenants.Commands;
using Application.UseCases.Auth.Users.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class AdminController(ISender mediator) : ControllerBase
{
    [HttpPost("create-role")]
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
    
    [HttpGet("get-all-roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
    {
        var roles = await mediator.Send(new GetAllRoles.Query());

        return Ok(roles);
    }
    
    [HttpGet("get-all-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var roles = await mediator.Send(new GetAllUsers.Query());

        return Ok(roles);
    }
}