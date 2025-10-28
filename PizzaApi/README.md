# PizzaApi

This project is a RESTful API for managing pizzas, designed following Domain-Driven Design (DDD) principles. It is structured into several layers, each with distinct responsibilities, promoting separation of concerns and maintainability.

## Project Structure

- **src/PizzaApi.Web**: Contains the presentation layer, including controllers that handle HTTP requests.
  - **Controllers/PizzaController.cs**: Manages pizza-related HTTP requests.
  - **Program.cs**: Entry point for the application, configuring services and middleware.

- **src/PizzaApi.Domain**: Represents the core domain logic.
  - **Entities/Pizza.cs**: Defines the `Pizza` entity with properties like `Id`, `Name`, `Description`, `Url`, `Price`, and a list of ingredients.
  - **ValueObjects/Size.cs**: Represents the size of pizzas (e.g., small, medium, large).
  - **Aggregates/OrderAggregate.cs**: Manages the relationship between pizzas and orders.
  - **Repositories/IPizzaRepository.cs**: Interface for pizza data access.

- **src/PizzaApi.Application**: Contains application logic and services.
  - **Services/PizzaService.cs**: Implements business logic for pizza operations.
  - **DTOs/PizzaDto.cs**: Data Transfer Object for transferring pizza data between layers.

- **src/PizzaApi.Infrastructure**: Handles data access and infrastructure concerns.
  - **Data/PizzaDbContext.cs**: Entity Framework Core database context for managing pizza entities.
  - **Repositories/PizzaRepository.cs**: Implements the `IPizzaRepository` interface for data access.

- **src/PizzaApi.Tests**: Contains unit tests for the application.
  - **PizzaServiceTests.cs**: Tests for the `PizzaService` class.

## Getting Started

To run the project, ensure you have the necessary dependencies installed. You can build and run the application using the following commands:

```bash
dotnet build
dotnet run --project src/PizzaApi.Web
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.