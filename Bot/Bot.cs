using Bot.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rcon;

namespace Bot;

public static class BotExtensions
{
    public static IHostBuilder UseDiscordBot(this IHostBuilder builder)
    {
        return builder.ConfigureServices((ctx, services) =>
        {
            var botSettings = ctx.Configuration.GetSection("Discord:Bot").Get<BotServiceConfig>();
            var discordSocketConfig = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = botSettings.AlwaysDownloadUsers,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                GatewayIntents = GatewayIntents.Guilds,
#if DEBUG
                LogLevel = LogSeverity.Debug
#else
                LogLevel = LogSeverity.Warning
#endif
            };

            services.AddSingleton(new DiscordSocketClient(discordSocketConfig))
                .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .AddSingleton(botSettings)
                .AddHostedService<BotService>();
        }).UseRcon();
    }
}