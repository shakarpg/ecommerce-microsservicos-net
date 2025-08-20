using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging.Contracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Sales.API.Messaging;

public class SalePublisher
{
    private readonly IConfiguration _config;
    public SalePublisher(IConfiguration config) => _config = config;

    public void Publish(SaleConfirmedEvent evt)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:HostName"] ?? "localhost"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var queueName = _config["RabbitMQ:Queue"] ?? "SaleConfirmedQueue";
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));
        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}
