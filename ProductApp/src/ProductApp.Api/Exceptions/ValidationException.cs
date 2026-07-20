namespace ProductApp.Api.Exceptions;

/// <summary>
/// Exception thrown for request input that fails validation
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
