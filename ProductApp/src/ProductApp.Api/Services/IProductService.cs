using ProductApp.Shared.Contracts;

namespace ProductApp.Api.Services;

/// <summary>
/// Interface for business logic for exposing product data to API consumers
/// </summary>
public interface IProductService
{
    IReadOnlyList<ProductSummaryDto> GetProductSummaries();

    /// <summary>
    /// It returns Product Details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ProductDetailDto GetProductDetail(int id);

    /// <summary>
    /// It returns Metrics
    /// </summary>
    /// <returns></returns>
    ProductMetricsDto GetMetrics();
}
