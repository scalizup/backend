using System.Net.Mime;
using Application.UseCases.Menu.Queries;
using Application.UseCases.MenuSort.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class MenuSortController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> CreateMenuSort([FromBody] CreateMenuSort.Command command)
    {
        return new ObjectResult(await mediator.Send(command))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMenuSort.MenuDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetMenuSort.MenuDto>> GetMenuSort(
        [FromQuery] GetMenuSort.Query query)
    {
        return Ok(await mediator.Send(query));
    }
}