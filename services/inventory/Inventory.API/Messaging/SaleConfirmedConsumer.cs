using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging.Contracts;
using Inventory.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Inventory.API.Messaging;

public class SaleConfirmedConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SaleConfirmedConsumer> _logger;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IModel? _channel;
    private string _queueName = "SaleConfirmedQueue";

    public SaleConfirmedConsumer(IServiceProvider sp, ILogger<SaleConfirmedConsumer> logger, IConfiguration config)
    {
        _sp = sp;
        _logger = logger;
        _config = config;
        _queueName = _config["RabbitMQ:Queue"] ?? _queueName;
        InitRabbit();
    }

    private void InitRabbit()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:HostName"] ?? "localhost"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var evt = JsonSerializer.Deserialize<SaleConfirmedEvent>(json);
            if (evt is null) return;

            _logger.LogInformation("Consuming SaleConfirmed: OrderId={OrderId}, ProductId={ProductId}, Qty={Qty}", evt.OrderId, evt.ProductId, evt.Quantity);
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == evt.ProductId, stoppingToken);
            if (product != null && product.Quantity >= evt.Quantity)
            {
                product.Quantity -= evt.Quantity;
                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Stock decreased for Product {ProductId}", evt.ProductId);
            }
            else
            {
                _logger.LogWarning("Insufficient stock for Product {ProductId}", evt.ProductId);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
