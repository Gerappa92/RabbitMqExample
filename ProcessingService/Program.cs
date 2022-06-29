// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessingService;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddConfiguration(configuration);
    })
    .ConfigureServices((_, services) =>
    {
        services.Configure<QueueOptions>(configuration.GetSection(QueueOptions.Location));
        services.AddHostedService<ProcessingService.DataCapture>();
        services.AddHostedService<ProcessData>();
    })
    .Build();

await host.StartAsync();

await host.WaitForShutdownAsync();

