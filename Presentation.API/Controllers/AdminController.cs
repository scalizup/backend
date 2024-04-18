using System.Net.Mime;
using Application.Models;
using Application.UseCases.Auth.Roles.Commands;
using Application.UseCases.Auth.Roles.Queries;
using Application.UseCases.Auth.Tenants.Commands;
using Application.UseCases.Auth.Users.Queries;
using Application.UseCases.Tenants.Commands;
using Application.UseCases.Tenants.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class AdminController(ISender mediator) : ControllerBase
{
    [HttpPost("create-tenant")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> CreateTenant([FromBody] CreateTenant.Command command)
    {
        var tenantCreated = await mediator.Send(command);

        return new ObjectResult(tenantCreated)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
    
    [HttpGet("get-all-tenants")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTenants.TenantDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTenants.TenantDto>>> GetAllTenants(
        [FromQuery] PageQuery pageQuery)
    {
        var tenants = await mediator.Send(new GetAllTenants.Query(pageQuery));

        return Ok(tenants);
    }
    
    
    [HttpPatch("update-tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTenant([FromBody] UpdateTenant.Command command)
    {
        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("delete-tenant/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteTenant([FromRoute] int id)
    {
        await mediator.Send(new DeleteTenant.Command(id));

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