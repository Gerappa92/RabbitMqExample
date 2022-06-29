using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProcessingService;

public class ProcessData : IHostedService
{
    private IConnection? _connection;
    private IModel? _channel;
    public ProcessData(IOptions<QueueOptions> queueOptions)
    {
        QueueName = queueOptions.Value.QueueName;
        FilePath = queueOptions.Value.ProcessDataFolderPath;
        FileExtension = queueOptions.Value.FileExtension;
    }

    public string QueueName { get; init; }
    public string FilePath { get; set; }
    public string FileExtension { get; init; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //todo:get hostname from configuration
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Directory.CreateDirectory(FilePath);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnReceived;
        _channel.BasicConsume(queue: QueueName,
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private void OnReceived(object? sender, BasicDeliverEventArgs e)
    {
        Console.WriteLine("Received new file!");
        var body = e.Body.ToArray();
        var path = Path.Combine(FilePath, Guid.NewGuid() + FileExtension);
        File.WriteAllBytes(path, body);
        Console.WriteLine("File created in process data folder {0}", path);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _connection?.Dispose();
        return Task.CompletedTask;
    }
}