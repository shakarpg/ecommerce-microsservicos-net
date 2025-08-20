namespace Inventory.API.DTOs;
public record CreateProductDto(string Name, string Description, decimal Price, int Quantity);
public record ProductDto(int Id, string Name, string Description, decimal Price, int Quantity);
