using Bot;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((app, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
              .AddJsonFile("appsettings.Development.json", true, true)
#else
              .AddJsonFile("appsettings.json", true, true)
#endif
              .AddEnvironmentVariables();

        Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(config.Build())
           .CreateLogger();
    })
    .ConfigureServices((ctx, services) =>
        services.AddSingleton(ctx.Configuration))
    .UseSerilog((ctx, config) =>
        config.ReadFrom.Configuration(ctx.Configuration))
    .UseDiscordBot()
    .Build();

host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Application is starting...");

await host.RunAsync();
