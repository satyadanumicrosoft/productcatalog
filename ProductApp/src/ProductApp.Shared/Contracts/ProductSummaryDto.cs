namespace ProductApp.Shared.Contracts;

/// <summary>
/// It contains product summary used by the catalog / list view.
/// Returned by GET /api/products.
/// </summary>
public class ProductSummaryDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string? Price { get; set; }

    public string? PriceUnit { get; set; }
}
