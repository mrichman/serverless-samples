using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using ServerlessTestSamples.Core.Services;
using ServerlessTestSamples.Core.Queries;
using Serilog;
using Serilog.Formatting.Compact;

namespace ServerlessTestSamples.Integrations;

public static class Startup
{
    public static ServiceProvider ServiceProvider { get; private set; }

    public static void InitializeServiceProvider()
    {
        var services = new ServiceCollection();

        var logger = new LoggerConfiguration()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();


        services.AddSingleton<AmazonS3Client>(new AmazonS3Client());
        services.AddTransient<IStorageService, StorageService>();
        services.AddSingleton<ListStorageAreasQueryHandler>();
        ServiceProvider = services.BuildServiceProvider();
    }
}