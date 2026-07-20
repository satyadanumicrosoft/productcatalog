namespace ProductApp.Shared.Contracts;

/// <summary>
/// It contain product record, including description and price.
/// Returned by GET /api/products/{id}.
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
