using CoreRCON;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Rcon;
public class RconService
{
    private readonly ILogger<RconService> _logger;
    private readonly RconServiceConfig _rconConfig;

    public RconService(ILogger<RconService> logger, RconServiceConfig rconConfig)
    {
        _logger = logger;
        _rconConfig = rconConfig;
    }

    public bool Connected { get; set; } = false;
    private RCON? _rcon;
    public RCON? Rcon
    {
        get
        {
            if (_rcon is null || Connected == false)
            {
                _rcon = Task.Run(async () => await ConnectAsync()).Result;
            }

            return _rcon;
        }
    }

    private async Task<RCON?> ConnectAsync()
    {
        RCON? rcon = null;
        try
        {
            rcon = new RCON(_rconConfig.IPAddress, _rconConfig.Port, _rconConfig.Password);

            // Connect to a server
            await rcon.ConnectAsync();

            _logger.LogDebug("RCON connected.");
            Connected = true;
            rcon.OnDisconnected += () =>
            {
                Connected = false;
                _logger.LogDebug("RCON disconnected.");
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RCON couldn't connect");
        }

        return rcon;
    }

    public async Task SendRconCommandAsync(SocketInteractionContext context, string? command)
    {
        if (Rcon is null)
        {
            _logger.LogError("{parameter} was null.", nameof(Rcon));
            return;
        }
        if (context is null)
        {
            _logger.LogError("{parameter} was null.", nameof(context));
            return;
        }

        _logger.LogDebug("RCON: BOT->SERVER \"{command}\"", command);
        var response = await Rcon.SendCommandAsync(command);
        _logger.LogDebug("RCON: SERVER->BOT: \"{response}\"", response);

        var lines = response.Split("\r\n");
        var reply = string.Empty;
        var replied = false;
        for (var i = 0; i < lines.Length; i++)
        {
            reply += lines[i] + "\r\n";
            if (i == lines.Length - 1 || reply.Length + lines[i + 1].Length > 1500)
            {

                if (!replied)
                {
                    await context.Interaction.RespondAsync(reply);
                    replied = true;
                }
                else
                {
                    await context.Channel.SendMessageAsync(reply);
                }
                reply = string.Empty;
            }
        }
    }

    public async Task ReadRconLogAsync(CancellationToken stoppingToken)
    {
        var log = new LogReceiver(50000, new IPEndPoint(_rconConfig.IPAddress, _rconConfig.Port));

        while (!stoppingToken.IsCancellationRequested)
        {
            log.ListenRaw(msg =>
            {
                _logger.LogInformation("RCON Listen: {chat}", msg);
            });

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
