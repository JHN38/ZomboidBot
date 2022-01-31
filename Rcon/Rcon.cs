using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Rcon;

public class RconServiceConfig
{
    public string? _host;
    public string? Host
    {
        get => _host; set
        {
            _host = value;
            Task.Run(async () => IPAddress = (await Dns.GetHostAddressesAsync(_host ?? IPAddress.Loopback.ToString())).SingleOrDefault() ?? IPAddress.Loopback);
        }
    }
    public IPAddress IPAddress { get; set; } = IPAddress.Loopback;
    public ushort Port { get; set; }
    public string? Password { get; set; }
}

public static class RconExtensions
{
    public static IHostBuilder UseRcon(this IHostBuilder builder)
    {
        return builder.ConfigureServices((ctx, services) =>
        {
            var config = ctx.Configuration.GetSection("Rcon:Zomboid").Get<RconServiceConfig>();

            services.ConfigureRconServices(config);
        });
    }
    public static IHostBuilder UseRcon(this IHostBuilder builder, Action<RconServiceConfig> configuration)
    {
        return UseRcon(builder, (ctx, config) => configuration(config));
    }
    public static IHostBuilder UseRcon(this IHostBuilder builder, Action<HostBuilderContext, RconServiceConfig> configuration)
    {
        return builder.ConfigureServices((ctx, services) =>
        {
            var config = new RconServiceConfig();
            configuration(ctx, config);

            services.ConfigureRconServices(config);
        });
    }
    private static IServiceCollection ConfigureRconServices(this IServiceCollection services, RconServiceConfig configuration)
    {
        return services.AddSingleton(configuration)
            .AddSingleton<RconService>();
    }
}
