using System.Text.Json.Serialization;

namespace ProductApp.Api.Models;

/// <summary>
/// Model entity to match Data/product-details.json summary detail
/// </summary>
public class ProductSummaryRecord
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}
