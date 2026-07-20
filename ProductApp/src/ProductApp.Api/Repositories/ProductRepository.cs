using System.Text.Json;
using ProductApp.Api.Models;

namespace ProductApp.Api.Repositories;

/// <summary>
/// It loads two JSON files (products.json and product-details.json) once at startup and serves them from memory.
/// Registered as a singleton so the files are parsed exactly once for the lifetime of the process
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;
    private readonly IReadOnlyList<ProductSummaryRecord> _summaries;
    private readonly IReadOnlyDictionary<int, ProductDetailRecord> _detailsById;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProductRepository(IWebHostEnvironment environment, ILogger<ProductRepository> logger)
    {
        _logger = logger;

        var summariesPath = Path.Combine(environment.ContentRootPath, "Data", "products.json");
        var detailsPath = Path.Combine(environment.ContentRootPath, "Data", "product-details.json");

        _summaries = LoadFromFile<ProductSummaryRecord>(summariesPath);

        var details = LoadFromFile<ProductDetailRecord>(detailsPath);
        _detailsById = details.ToDictionary(d => d.Id);

        _logger.LogInformation(
            "Product catalog loaded: {SummaryCount} summaries, {DetailCount} detail records",
            _summaries.Count, _detailsById.Count);
    }

    public IReadOnlyList<ProductSummaryRecord> GetSummaries() => _summaries;

    public ProductDetailRecord? GetDetailById(int id) =>
        _detailsById.TryGetValue(id, out var record) ? record : null;

    public IReadOnlyList<ProductDetailRecord> GetAllDetails() => _detailsById.Values.ToList();

    private List<T> LoadFromFile<T>(string path)
    {
        if (!File.Exists(path))
        {
            _logger.LogError("Expected data file not found at {Path}", path);
            throw new FileNotFoundException($"Required data file not found: {path}");
        }

        using var stream = File.OpenRead(path);
        var items = JsonSerializer.Deserialize<List<T>>(stream, SerializerOptions);

        return items ?? new List<T>();
    }
}
