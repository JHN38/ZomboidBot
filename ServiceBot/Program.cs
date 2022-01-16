using Serilog;
using ServiceBot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((app, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", true, true)
              .AddEnvironmentVariables();

        Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(config.Build())
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .CreateLogger();

        if (app.HostingEnvironment.IsDevelopment())
        {
            // dev only
        }
        else
        {
            // prod only
        }
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .UseSerilog()
    .Build();

Log.Logger.Information("Application is starting...");

await host.RunAsync();
