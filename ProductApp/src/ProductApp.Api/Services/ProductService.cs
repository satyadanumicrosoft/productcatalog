using System.Globalization;
using System.Text.RegularExpressions;
using ProductApp.Api.Exceptions;
using ProductApp.Api.Models;
using ProductApp.Api.Repositories;
using ProductApp.Shared.Contracts;

namespace ProductApp.Api.Services;

/// <summary>
/// ProductService for exposing product data to API consumers
/// </summary>
public partial class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IReadOnlyList<ProductSummaryDto> GetProductSummaries()
    {
        var summaries = _repository.GetSummaries();
        var detailsById = _repository.GetAllDetails().ToDictionary(d => d.Id);

        _logger.LogInformation("Returning {Count} product summaries", summaries.Count);

        return summaries
            .Select(s => MapToSummaryDto(s, detailsById.GetValueOrDefault(s.Id)))
            .ToList();
    }

    public ProductDetailDto GetProductDetail(int id)
    {
        if (id < 0)
        {
            throw new ValidationException("Product id must be a non-negative integer.");
        }

        var record = _repository.GetDetailById(id);

        if (record is null)
        {
            _logger.LogWarning("Product {ProductId} was requested but does not exist", id);
            throw new ProductNotFoundException(id);
        }

        return MapToDetailDto(record);
    }

    public ProductMetricsDto GetMetrics()
    {
        var details = _repository.GetAllDetails();

        if (details.Count == 0)
        {
            _logger.LogWarning("Metrics requested but the catalog is empty");
            return new ProductMetricsDto
            {
                TotalProducts = 0,
                AveragePrice = 0,
                MostExpensive = null,
                LeastExpensive = null,
                ByPriceUnit = new Dictionary<string, int>()
            };
        }

        var parsed = details
            .Select(d => new { Record = d, Parsed = TryParsePrice(d.Price) })
            .Where(x => x.Parsed is not null)
            .Select(x => (x.Record, Amount: x.Parsed!.Value.Amount, Unit: x.Parsed!.Value.Unit))
            .ToList();

        if (parsed.Count == 0)
        {
            _logger.LogWarning("No product prices could be parsed while computing metrics");
        }

        var mostExpensive = parsed.OrderByDescending(x => x.Amount).FirstOrDefault();
        var leastExpensive = parsed.OrderBy(x => x.Amount).FirstOrDefault();

        var metrics = new ProductMetricsDto
        {
            TotalProducts = details.Count,
            AveragePrice = parsed.Count > 0
                ? Math.Round(parsed.Average(x => x.Amount), 2)
                : 0,
            MostExpensive = mostExpensive.Record is null ? null : MapToReferenceDto(mostExpensive.Record, mostExpensive.Record.Price),
            LeastExpensive = leastExpensive.Record is null ? null : MapToReferenceDto(leastExpensive.Record, leastExpensive.Record.Price),
            ByPriceUnit = parsed
                .GroupBy(x => x.Unit)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        _logger.LogInformation(
            "Computed metrics for {Count} products, average price {AveragePrice:C}",
            metrics.TotalProducts, metrics.AveragePrice);

        return metrics;
    }

    private static ProductSummaryDto MapToSummaryDto(ProductSummaryRecord record, ProductDetailRecord? matchingDetail)
    {
        var parsedPrice = matchingDetail is not null ? TryParsePrice(matchingDetail.Price) : null;

        return new ProductSummaryDto
        {
            Id = record.Id,
            Title = record.Title,
            Summary = record.Summary,
            ImageUrl = record.ImageUrl,
            Price = matchingDetail?.Price,
            PriceUnit = parsedPrice?.Unit
        };
    }

    private static ProductDetailDto MapToDetailDto(ProductDetailRecord record) => new()
    {
        Id = record.Id,
        Title = record.Title,
        Summary = record.Summary,
        Description = record.Description,
        Price = record.Price,
        ImageUrl = record.ImageUrl
    };

    private static ProductReferenceDto MapToReferenceDto(ProductDetailRecord record, string price) => new()
    {
        Id = record.Id,
        Title = record.Title,
        Price = price
    };

    /// <summary>
    /// It parses a price string such as "$0.59/lb" into a numeric amount (0.59) and a unit suffix ("/lb"). 
    /// Returns null if the string doesn't match the expected pattern
    /// </summary>
    private static (decimal Amount, string Unit)? TryParsePrice(string price)
    {
        if (string.IsNullOrWhiteSpace(price))
        {
            return null;
        }

        var match = PriceRegex().Match(price);
        if (!match.Success)
        {
            return null;
        }

        if (!decimal.TryParse(match.Groups["amount"].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            return null;
        }

        var unit = match.Groups["unit"].Success ? match.Groups["unit"].Value : "unspecified";

        return (amount, unit);
    }

    [GeneratedRegex(@"^\$(?<amount>[0-9]+(?:\.[0-9]+)?)(?<unit>/.+)?$")]
    private static partial Regex PriceRegex();
}
