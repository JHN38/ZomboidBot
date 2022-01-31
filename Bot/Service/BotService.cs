//
// https://siderite.dev/blog/creating-discord-chat-bot-in-net/#at163758227
//

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bot.Services;

public class BotService : BackgroundService
{
    private readonly ILogger<BotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly CommandHandler _handler;
    private readonly BotServiceConfig _botSettings;

    public BotService(ILogger<BotService> logger, DiscordSocketClient client, InteractionService commands, CommandHandler handler, BotServiceConfig botSettings)
    {
        _logger = logger;
        _client = client;
        _commands = commands;
        _handler = handler;
        _botSettings = botSettings;
    }

    // Service starting
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceived;
            _client.Ready += ReadyAsync;

            _commands.Log += LogAsync;

            await _client.LoginAsync(TokenType.Bot, _botSettings.Token);
            await _client.StartAsync();

            await _handler.InitializeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Exception occured while starting the service.");
        }

        await base.StartAsync(cancellationToken);
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        try
        {
            var channel = (IChannel)arg.Channel;
            var guild = (IGuild)((SocketGuildChannel)channel).Guild;
            var author = (IUser)arg.Author;
            var message = (IMessage)arg;
            var msg = message.Content;

            foreach (var mentionedUser in message.MentionedUserIds)
            {
                var user = (IUser)await guild.GetUserAsync(mentionedUser);
                msg = msg.Replace(user.Mention, user.Username);
            }

            await Task.Run(() =>
            {
                using (_logger.BeginScope("MessageReceived"))
                {
                    _logger.LogInformation("'{guild}' => #{channel} <{user}> {message}", guild.Name, channel.Name, author.Username, msg);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Exception occured in MessageReceived.");
        }
    }

    private async Task ReadyAsync()
    {
#if DEBUG
        // this is where you put the id of the test discord guild
        _logger.LogInformation("Bot ready in Debug mode...");

        if (_botSettings.GuildId > 0)
        {
            foreach(var module in _commands.Modules)
            {
                foreach (var command in module.SlashCommands)
                {
                    _logger.LogDebug("Registering {type} command \"{command}\" from module \"{module}\".", command.CommandType, command.Name, module.Name);
                }
                foreach (var command in module.ContextCommands)
                {
                    _logger.LogDebug("Registering {type} command \"{command}\" from module \"{module}\".", command.CommandType, command.Name, module.Name);
                }
            }

            await _commands.RegisterCommandsToGuildAsync(_botSettings.GuildId, true);
        }
#else
        _logger.LogInformation("Bot ready in Production mode...");
        //await _commands.RegisterCommandsGloballyAsync(true);
#endif

        _logger.LogInformation("Connected as -> [{currentUser}] :)", _client.CurrentUser.Username);

        await _client.SetGameAsync("with your mind");
    }

    // Service running
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    // Service stopping
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _client.SetGameAsync(null);
            await _client.StopAsync();

            _client.Log -= LogAsync;
            _client.Ready -= ReadyAsync;
            _client.MessageReceived -= MessageReceived;
            _commands.Log -= LogAsync;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Exception occured while stopping the service.");
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task LogAsync(LogMessage arg)
    {
        await Task.Run(() =>
        {
            using (_logger.BeginScope("Log"))
            {
                _logger.Log(LogLevelConverter.Convert(arg.Severity), arg.Exception, "LOG: {message}", arg.Message ?? arg.Exception?.Message);
            }
        });
    }
}
