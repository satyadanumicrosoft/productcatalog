namespace ProductApp.Shared.Contracts;

/// <summary>
/// Error response returned by the API for every non-2xx response.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>Machine friendly error code.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Human readable explanation for the end user.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>HTTP status code.</summary>
    public int StatusCode { get; set; }

    /// <summary>Correlation id that ties this error back to a server log entry.</summary>
    public string TraceId { get; set; } = string.Empty;
}
