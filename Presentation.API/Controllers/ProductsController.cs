using System.Net.Mime;
using Application.Models;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using Presentation.API.Services;

namespace Presentation.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[Route("api/[controller]")]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateProduct(
        [FromForm] CreateProduct.Command command,
        [FromForm] IFormFile? image)
    {
        if (image is not null)
        {
            command.Image = new Image(image);
        }

        var createdProductId = await mediator.Send(command);

        return new ObjectResult(createdProductId)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageQueryResponse<GetAllProducts.ProductDto>))]
    public async Task<ActionResult<PageQueryResponse<GetAllProducts.ProductDto>>> GetAllProducts(
        [FromQuery] PageQuery pageQuery)
    {
        var products = await mediator.Send(new GetAllProducts.Query(pageQuery));

        return new ObjectResult(products)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateProduct(
        [FromForm] UpdateProduct.Command command,
        [FromForm] IFormFile? image)
    {
        if (image is not null)
        {
            command.Image = new Image(image);
        }

        await mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct([FromRoute] int id)
    {
        await mediator.Send(new DeleteProduct.Command(id));

        return NoContent();
    }
}