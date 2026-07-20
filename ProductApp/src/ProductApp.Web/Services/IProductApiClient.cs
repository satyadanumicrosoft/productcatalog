using ProductApp.Shared.Contracts;

namespace ProductApp.Web.Services;

/// <summary>
/// Pages depend on interface IProductApiClient rather than HttpClient directly
/// </summary>
public interface IProductApiClient
{
    Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// It returns the product detail
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductDetailDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// It return Metrics
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProductMetricsDto> GetMetricsAsync(CancellationToken cancellationToken = default);
}
