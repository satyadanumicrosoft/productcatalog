using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using ProductApp.Shared.Contracts;
using ProductApp.Web.Exceptions;

namespace ProductApp.Web.Services;

/// <summary>
/// It uses a typed HttpClient (registered and configured once in Program.cs)
/// </summary>
public class ProductApiClient : IProductApiClient
{
    private const string ProductsRoute = "api/products";

    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductApiClient> _logger;
    private readonly IHostEnvironment _environment;

    public ProductApiClient(HttpClient httpClient, ILogger<ProductApiClient> logger, IHostEnvironment environment)
    {
        _httpClient = httpClient;
        _logger = logger;
        _environment = environment;
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductSummaryDto>>(ProductsRoute, cancellationToken);
            return result ?? new List<ProductSummaryDto>();
        }
        catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
        {
            _logger.LogError(ex, "Failed to load product list from {Route} (base address {BaseAddress})", ProductsRoute, _httpClient.BaseAddress);
            throw new ProductApiException(BuildErrorMessage("The product catalog could not be loaded.", ex), ex);
        }
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ProductsRoute}/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductDetailDto>(cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
        {
            _logger.LogError(ex, "Failed to load product {ProductId} from the API (base address {BaseAddress})", id, _httpClient.BaseAddress);
            throw new ProductApiException(BuildErrorMessage($"Product {id} could not be loaded.", ex), ex);
        }
    }

    public async Task<ProductMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ProductMetricsDto>($"{ProductsRoute}/metrics", cancellationToken);
            return result ?? new ProductMetricsDto();
        }
        catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
        {
            _logger.LogError(ex, "Failed to load metrics from the API (base address {BaseAddress})", _httpClient.BaseAddress);
            throw new ProductApiException(BuildErrorMessage("Catalog metrics could not be loaded.", ex), ex);
        }
    }

    /// <summary>
    /// It builds generic error messages
    /// </summary>
    /// <param name="userFacingMessage"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private string BuildErrorMessage(string userFacingMessage, Exception ex)
    {
        if (!_environment.IsDevelopment())
        {
            return $"{userFacingMessage} Please try again shortly.";
        }

        var apiBaseAddress = _httpClient.BaseAddress?.ToString() ?? "(no base address configured)";
        return $"{userFacingMessage} [DEV] Calling {apiBaseAddress} failed: {ex.Message}";
    }
}
