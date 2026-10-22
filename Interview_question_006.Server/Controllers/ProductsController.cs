using Example.InterviewQuestion006.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Example.InterviewQuestion006.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ProductService productService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ProductDto>> GetAll(CancellationToken cancellationToken) =>
        productService.GetAllAsync(cancellationToken);

    [HttpPost]
    [ProducesResponseType<ProductDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetBarcode), new { id = product.Id }, product);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:int}/barcode")]
    [Produces("image/svg+xml")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBarcode(int id, CancellationToken cancellationToken)
    {
        var svg = await productService.GetBarcodeSvgAsync(id, cancellationToken);
        return Content(svg, "image/svg+xml");
    }
}
