namespace ProductApp.Shared.Contracts;

/// <summary>
/// It aggregate statistics about the product catalog
/// Returned by GET /api/products/metrics.
/// </summary>
public class ProductMetricsDto
{
    public int TotalProducts { get; set; }

    public decimal AveragePrice { get; set; }

    public ProductReferenceDto? MostExpensive { get; set; }

    public ProductReferenceDto? LeastExpensive { get; set; }
    
    public Dictionary<string, int> ByPriceUnit { get; set; } = new();
}
