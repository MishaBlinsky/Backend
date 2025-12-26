using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Consumer.Config;
using System.Text;
using Common;
using Backend.BBL.Models;
using Backend.Clients;
using Backend.DAL.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Consumer.Consumers
{
    public class OmsOrderCreatedConsumer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<RabbitMqSettings> _rabbitMqSettings;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private AsyncEventingBasicConsumer _consumer;
        public OmsOrderCreatedConsumer(IOptions<RabbitMqSettings> rabbitMqSettings, IServiceProvider serviceProvider)
        {
            _rabbitMqSettings = rabbitMqSettings;
            _serviceProvider = serviceProvider;
            _factory = new ConnectionFactory { HostName = rabbitMqSettings.Value.HostName, Port = rabbitMqSettings.Value.Port };
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = await _factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await _channel.QueueDeclareAsync(
                queue: _rabbitMqSettings.Value.OrderCreatedQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = message.FromJson<OmsOrderCreatedMessage>();
                Console.WriteLine("Received: " + message);
                using var scope = _serviceProvider.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<OmsClient>();
                await client.LogOrder(new V1AuditLogOrderRequest
                {
                    Orders = order.OrderItems.Select(x =>
                        new V1AuditLogOrderRequest.LogOrder
                        {
                            OrderId = order.Id,
                            OrderItemId = x.Id,
                            CustomerId = order.CustomerId,
                            OrderStatus = nameof(OrderStatus.Created)
                        }).ToArray()
                }, CancellationToken.None);
            };
            await _channel.BasicConsumeAsync(
                queue: _rabbitMqSettings.Value.OrderCreatedQueue,
                autoAck: true,
                consumer: _consumer,
                cancellationToken: cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
    public enum OrderStatus
    {
        Created, Processing, Completed, Cancelled
    }
}