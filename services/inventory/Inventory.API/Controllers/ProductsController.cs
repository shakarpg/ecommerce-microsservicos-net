using Inventory.API.Data;
using Inventory.API.DTOs;
using Inventory.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly InventoryDbContext _db;
    public ProductsController(InventoryDbContext db) => _db = db;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var items = await _db.Products
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Quantity))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto input)
    {
        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Quantity = input.Quantity
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Quantity));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p is null) return NotFound();
        return new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Quantity);
    }

    [HttpGet("validate/{productId:int}/{quantity:int}")]
    [Authorize]
    public async Task<IActionResult> ValidateStock(int productId, int quantity)
    {
        var p = await _db.Products.FindAsync(productId);
        if (p is null) return NotFound(new { message = "Produto não encontrado" });
        return Ok(new { available = p.Quantity >= quantity });
    }

    [HttpPost("decrease-stock")]
    [Authorize]
    public async Task<IActionResult> DecreaseStock([FromBody] Dictionary<int,int> productQuantities)
    {
        foreach (var kv in productQuantities)
        {
            var product = await _db.Products.FindAsync(kv.Key);
            if (product is null) return NotFound(new { message = $"Produto {kv.Key} não encontrado" });
            if (product.Quantity < kv.Value) return BadRequest(new { message = $"Estoque insuficiente para produto {kv.Key}" });
            product.Quantity -= kv.Value;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }
}
