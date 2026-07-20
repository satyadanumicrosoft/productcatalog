namespace ProductApp.Shared.Contracts;

/// <summary>
/// It contains Id, Title and Price
/// </summary>
public class ProductReferenceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
}
