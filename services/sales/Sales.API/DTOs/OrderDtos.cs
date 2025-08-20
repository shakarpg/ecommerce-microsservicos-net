namespace Sales.API.DTOs;
public record OrderItemInput(int ProductId, int Quantity);
public record CreateOrderDto(List<OrderItemInput> Items);
public record OrderDto(int Id, string Status, DateTime CreatedAt, IEnumerable<OrderItemOutput> Items);
public record OrderItemOutput(int ProductId, int Quantity, decimal UnitPrice);
