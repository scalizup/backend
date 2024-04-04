using System.Net.Mime;
using Application.UseCases.Auth.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RegisterUser([FromBody] RegisterUser.Command command)
    {
        return new ObjectResult(await mediator.Send(command))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginUser.Dto>> LoginUser([FromBody] LoginUser.Command command)
    {
        return Ok(await mediator.Send(command));
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginUser.Dto>> RefreshToken([FromBody] RefreshUserToken.Command command)
    {
        return Ok(await mediator.Send(command));
    }
    
}