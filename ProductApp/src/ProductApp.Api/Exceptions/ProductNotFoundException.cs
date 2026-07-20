namespace ProductApp.Api.Exceptions;

/// <summary>
/// It returns NotFound Exception
/// </summary>
public class ProductNotFoundException : Exception
{
    public int ProductId { get; }

    public ProductNotFoundException(int productId)
        : base($"Product with id '{productId}' was not found.")
    {
        ProductId = productId;
    }
}
