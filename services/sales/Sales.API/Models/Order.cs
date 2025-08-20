namespace Sales.API.Models;
public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Confirmed";
    public List<OrderItem> Items { get; set; } = new();
}
