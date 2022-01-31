namespace Bot;
public class BotServiceConfig
{
    public bool AlwaysDownloadUsers { get; set; }
    public string? Token { get; set; }
    public ulong GuildId { get; set; } = 0x0;
}
