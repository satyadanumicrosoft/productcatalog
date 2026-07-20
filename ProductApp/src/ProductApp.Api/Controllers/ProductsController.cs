using Microsoft.AspNetCore.Mvc;
using ProductApp.Api.Services;
using ProductApp.Shared.Contracts;

namespace ProductApp.Api.Controllers;

/// <summary>
/// ProductsController contains endpoints for Product catalog
/// </summary>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>Returns the product catalog list (id, title, summary, imageUrl).</summary>
    /// <response code="200">The catalog list, possibly empty.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductSummaryDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ProductSummaryDto>> GetProducts()
    {
        _logger.LogInformation("GET /api/products called");

        var summaries = _productService.GetProductSummaries();

        return Ok(summaries);
    }

    /// <summary>Returns the full detail record for a single product.</summary>
    /// <param name="id">The product id.</param>
    /// <response code="200">The product detail.</response>
    /// <response code="400">The id was not a valid non-negative integer.</response>
    /// <response code="404">No product exists with the given id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ProductDetailDto> GetProductById([FromRoute] int id)
    {
        _logger.LogInformation("GET /api/products/{ProductId} called", id);
        
        var detail = _productService.GetProductDetail(id);

        return Ok(detail);
    }

    /// <summary>Returns aggregate metrics derived from the catalog (counts, pricing, breakdowns).</summary>
    /// <response code="200">The computed metrics.</response>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ProductMetricsDto), StatusCodes.Status200OK)]
    public ActionResult<ProductMetricsDto> GetMetrics()
    {
        _logger.LogInformation("GET /api/products/metrics called");

        var metrics = _productService.GetMetrics();

        return Ok(metrics);
    }
}
