using System.Net.Mime;
using Domain.UseCases.Tenant.Commands;
using Domain.UseCases.Tenant.Queries;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class TenantController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateTenant([FromBody] CreateTenant.Command command)
    {
        var newTenantId = await mediator.Send(command);

        return CreatedAtAction(nameof(GetTenantById), new { id = newTenantId }, newTenantId);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTenantById.TenantDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetTenantById.TenantDto>> GetTenantById([FromRoute] int id)
    {
        var tenant = await mediator.Send(new GetTenantById.Query(id));

        return Ok(tenant);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllTenants.TenantDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PageQueryResponse<GetAllTenants.TenantDto>>> GetAllTenants(
        [FromQuery] PageQuery pageQuery)
    {
        var tenants = await mediator.Send(new GetAllTenants.Query(pageQuery));

        return Ok(tenants);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateTenant([FromBody] UpdateTenant.Command command)
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
        await mediator.Send(new DeleteTenant.Command(id));

        return NoContent();
    }
}