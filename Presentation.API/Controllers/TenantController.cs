using System.Net.Mime;
using Application.UseCases.Tenants.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class TenantController(ISender mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTenantById.TenantDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetTenantById.TenantDto>> GetTenantById([FromRoute] int id)
    {
        var tenant = await mediator.Send(new GetTenantById.Query
        {
            TenantId = id
        });

        return Ok(tenant);
    }
}