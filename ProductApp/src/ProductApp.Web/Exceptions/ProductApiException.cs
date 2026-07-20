namespace ProductApp.Web.Exceptions;

/// <summary>
/// Exception thrown by ProductApiClient when a call to the
/// backend API fails, so Razor pages can catch a single known exception
/// </summary>
public class ProductApiException : Exception
{
    public ProductApiException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
