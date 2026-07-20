# Meijer Product Catalog

A .NET 8 product catalog application created for the STME Take-Home Assessment.

## Projects

* **ProductApp.Api** — ASP.NET Core Web API
* **ProductApp.Web** — Blazor Server web application
* **ProductApp.Shared** — Shared DTOs and contracts
* **ProductApp.Api.Tests** — xUnit service-layer tests

```text
ProductApp/
├── ProductApp.sln
├── src/
│   ├── ProductApp.Shared/
│   ├── ProductApp.Api/
│   └── ProductApp.Web/
└── tests/
    └── ProductApp.Api.Tests/
```

## Requirements

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

## Run the Application

Start the API in other terminal

Start the web application in another terminal

Application URLs:

* API: `https://localhost:7200`
* Swagger: `https://localhost:7200/swagger`
* Web application: `https://localhost:7100`

## API Endpoints

| Method | Endpoint                | Description                     |
| ------ | ----------------------- | ------------------------------- |
| GET    | `/api/products`         | Returns all products            |
| GET    | `/api/products/{id}`    | Returns product details         |
| GET    | `/api/products/metrics` | Returns catalog pricing metrics |

The API uses the provided `products.json` and `product-details.json` files as its only data source.

## Features

### API

* Product listing and product details
* Product pricing metrics
* Input validation
* Centralized exception handling
* Structured logging
* Dependency injection
* Swagger documentation
* Configured CORS policy

### Web Application

* Product overview dashboard
* Product search and filtering
* Product detail page
* Responsive design
* Product price-unit chart
* Add-to-List and sharing functionality
* Geolocation-based city information
* Clipboard fallback when browser sharing is unavailable

## Architecture

The API follows a layered architecture:

```text
Controller → Service → Repository
```

* **Controller** handles HTTP requests and responses.
* **Service** contains validation, mapping, and business logic.
* **Repository** loads and manages the JSON data.
* **Shared project** contains DTOs used by both the API and web application.

The web application uses `IProductApiClient` to communicate with the API instead of calling `HttpClient` directly from pages.

## Styling

The application uses Meijer blue (`#0c60a5`) as the primary brand color.

The interface is responsive and supports desktop, tablet, and mobile screen sizes.

## Testing

Run the unit 

The tests cover the `ProductService` business logic using an in-memory repository.
