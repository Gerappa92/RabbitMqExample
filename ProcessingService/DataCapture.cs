using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ProcessingService;

public class DataCapture : IHostedService
{
    private IConnection? _connection;
    private IModel? _channel;
    private FileSystemWatcher? _watcher;

    public DataCapture(IOptions<QueueOptions> queueOptions)
    {
        QueueName = queueOptions.Value.QueueName;
        FilePath = queueOptions.Value.DataCaptureFolderPath;
        FileExtension = queueOptions.Value.FileExtension;
    }

    public string QueueName { get; init; }
    public string FilePath { get; init; }
    public string FileExtension { get; init; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Directory.CreateDirectory(FilePath);

        _watcher = new FileSystemWatcher(FilePath);

        _watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;

        _watcher.Created += OnCreated;

        _watcher.Filter = $"*{FileExtension}";
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine("New file created: " + e.FullPath);
        var fileBinary = File.ReadAllBytes(e.FullPath);
        _channel.BasicPublish(exchange: "",
            routingKey: QueueName,
            basicProperties: null,
            body: fileBinary);
        Console.WriteLine("New file added to the queue {0}", QueueName);

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher?.Dispose();
        return Task.CompletedTask;
    }
}