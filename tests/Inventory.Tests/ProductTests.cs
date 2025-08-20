using Inventory.API.Models;
using Xunit;

public class ProductTests
{
    [Fact]
    public void CreateProduct_ShouldSetProperties()
    {
        var p = new Product { Name = "Test", Description = "D", Price = 10m, Quantity = 5 };
        Assert.Equal("Test", p.Name);
        Assert.Equal(5, p.Quantity);
    }
}
