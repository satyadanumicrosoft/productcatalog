using ProductApp.Api.Models;
using ProductApp.Api.Repositories;

namespace ProductApp.Api.Tests;

/// <summary>
/// Unit tests for ProductRepository of Product Summary and Details
/// </summary>
public class FakeProductRepository : IProductRepository
{
    private readonly List<ProductSummaryRecord> _summaries;
    private readonly List<ProductDetailRecord> _details;

    public FakeProductRepository(List<ProductSummaryRecord> summaries, List<ProductDetailRecord> details)
    {
        _summaries = summaries;
        _details = details;
    }

    public IReadOnlyList<ProductSummaryRecord> GetSummaries() => _summaries;

    public ProductDetailRecord? GetDetailById(int id) => _details.FirstOrDefault(d => d.Id == id);

    public IReadOnlyList<ProductDetailRecord> GetAllDetails() => _details;
}
