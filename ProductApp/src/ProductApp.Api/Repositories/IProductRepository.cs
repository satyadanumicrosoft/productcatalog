using ProductApp.Api.Models;

namespace ProductApp.Api.Repositories;

/// <summary>
/// Interface for Product Repository for Data access abstraction for product records.
/// </summary>
public interface IProductRepository
{
    /// <summary>Returns the lightweight catalog list (id, title, summary, imageUrl).</summary>
    IReadOnlyList<ProductSummaryRecord> GetSummaries();

    /// <summary>Returns the full detail record for a single product, or null if it doesn't exist.</summary>
    ProductDetailRecord? GetDetailById(int id);

    /// <summary>Returns every detail record, used by the service layer to compute catalog-wide metrics.</summary>
    IReadOnlyList<ProductDetailRecord> GetAllDetails();
}
