using System.Net.Http.Headers;
using System.Text.Json;
using BuildingBlocks.Messaging.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.DTOs;
using Sales.API.Messaging;
using Sales.API.Models;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly SalesDbContext _db;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _config;

    public OrdersController(SalesDbContext db, IHttpClientFactory clientFactory, IConfiguration config)
    {
        _db = db;
        _clientFactory = clientFactory;
        _config = config;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _db.Orders.Include(o => o.Items).ToListAsync();
        var result = orders.Select(o => new OrderDto(
            o.Id, o.Status, o.CreatedAt, o.Items.Select(i => new OrderItemOutput(i.ProductId, i.Quantity, i.UnitPrice))
        ));
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateOrderDto input)
    {
        if (input.Items is null || input.Items.Count == 0)
            return BadRequest(new { message = "Pedido sem itens." });

        // 1) valida com Inventory via HTTP (Gateway)
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var client = _clientFactory.CreateClient("Inventory");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        foreach (var item in input.Items)
        {
            var resp = await client.GetAsync($"/products/validate/{item.ProductId}/{item.Quantity}");
            if (!resp.IsSuccessStatusCode)
                return BadRequest(new { message = "Falha ao validar estoque." });

            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.GetProperty("available").GetBoolean())
                return BadRequest(new { message = $"Estoque insuficiente para produto {item.ProductId}" });
        }

        // 2) cria pedido
        var order = new Order();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            _db.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = 0m // preço poderia vir do catálogo
            });
        }
        await _db.SaveChangesAsync();

        // 3) publica evento para reduzir estoque de forma assíncrona
        var publisher = new SalePublisher(_config);
        foreach (var item in input.Items)
        {
            publisher.Publish(new SaleConfirmedEvent(order.Id, item.ProductId, item.Quantity));
        }

        return CreatedAtAction(nameof(GetAll), new { id = order.Id }, new { orderId = order.Id, status = order.Status });
    }
}
