namespace BuildingBlocks.Messaging.Contracts;
public record SaleConfirmedEvent(int OrderId, int ProductId, int Quantity);
