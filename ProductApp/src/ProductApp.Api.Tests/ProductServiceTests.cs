using Microsoft.Extensions.Logging.Abstractions;
using ProductApp.Api.Exceptions;
using ProductApp.Api.Models;
using ProductApp.Api.Services;
using Xunit;

namespace ProductApp.Api.Tests;

public class ProductServiceTests
{
    private static ProductService CreateService(
        List<ProductSummaryRecord>? summaries = null,
        List<ProductDetailRecord>? details = null)
    {
        var repository = new FakeProductRepository(
            summaries ?? new List<ProductSummaryRecord>(),
            details ?? new List<ProductDetailRecord>());

        return new ProductService(repository, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public void GetProductSummaries_MapsFieldsAndJoinsPrice()
    {
        var summaries = new List<ProductSummaryRecord>
        {
            new() { Id = 1, Title = "Bananas", Summary = "Fresh bananas", ImageUrl = "img.png" }
        };
        var details = new List<ProductDetailRecord>
        {
            new() { Id = 1, Title = "Bananas", Summary = "Fresh bananas", Description = "desc", Price = "$0.59/lb", ImageUrl = "img.png" }
        };

        var service = CreateService(summaries, details);

        var result = service.GetProductSummaries();

        Assert.Single(result);
        Assert.Equal("Bananas", result[0].Title);
        Assert.Equal("$0.59/lb", result[0].Price);
        Assert.Equal("/lb", result[0].PriceUnit);
    }

    [Fact]
    public void GetProductDetail_ReturnsMappedDto_WhenProductExists()
    {
        var details = new List<ProductDetailRecord>
        {
            new() { Id = 5, Title = "Carrots", Summary = "Crunchy", Description = "desc", Price = "$0.99/lb", ImageUrl = "img.png" }
        };
        var service = CreateService(details: details);

        var result = service.GetProductDetail(5);

        Assert.Equal("Carrots", result.Title);
        Assert.Equal("$0.99/lb", result.Price);
    }

    [Fact]
    public void GetProductDetail_ThrowsProductNotFoundException_WhenIdMissing()
    {
        var service = CreateService();

        var exception = Assert.Throws<ProductNotFoundException>(() => service.GetProductDetail(999));

        Assert.Equal(999, exception.ProductId);
    }

    [Fact]
    public void GetProductDetail_ThrowsValidationException_WhenIdIsNegative()
    {
        var service = CreateService();

        Assert.Throws<ValidationException>(() => service.GetProductDetail(-1));
    }

    [Fact]
    public void GetMetrics_ComputesAveragePriceMinMaxAndUnitBreakdown()
    {
        var details = new List<ProductDetailRecord>
        {
            new() { Id = 0, Title = "Bananas", Price = "$0.59/lb" },
            new() { Id = 1, Title = "Pineapples", Price = "$3.99/each" },
            new() { Id = 2, Title = "Garlic", Price = "$0.49/each" },
        };
        var service = CreateService(details: details);

        var metrics = service.GetMetrics();

        Assert.Equal(3, metrics.TotalProducts);
        Assert.Equal(1.69m, metrics.AveragePrice);
        Assert.Equal("Pineapples", metrics.MostExpensive?.Title);
        Assert.Equal("Garlic", metrics.LeastExpensive?.Title);
        Assert.Equal(2, metrics.ByPriceUnit["/each"]);
        Assert.Equal(1, metrics.ByPriceUnit["/lb"]);
    }

    [Fact]
    public void GetMetrics_ReturnsZeroedResult_WhenCatalogIsEmpty()
    {
        var service = CreateService();

        var metrics = service.GetMetrics();

        Assert.Equal(0, metrics.TotalProducts);
        Assert.Equal(0, metrics.AveragePrice);
        Assert.Null(metrics.MostExpensive);
        Assert.Null(metrics.LeastExpensive);
        Assert.Empty(metrics.ByPriceUnit);
    }
}
