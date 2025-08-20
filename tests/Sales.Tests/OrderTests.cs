using Sales.API.Models;
using Xunit;

public class OrderTests
{
    [Fact]
    public void NewOrder_ShouldHaveDefaultStatusConfirmed()
    {
        var o = new Order();
        Assert.Equal("Confirmed", o.Status);
    }
}
