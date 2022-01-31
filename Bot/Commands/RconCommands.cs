using Bot.Services;
using Discord;
using Discord.Interactions;
using Rcon;

namespace Bot.Modules
{
    // interation modules must be public and inherit from an IInterationModuleBase
    [Group("zomboid", "Zomboid Server commands.")]
    public class RconCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly RconService _rcon;

        public InteractionService? Commands { get; set; }
        public CommandHandler? Handler { get; set; }

        // constructor injection is also a valid way to access the dependecies
        public RconCommands(CommandHandler handler, RconService rcon)
        {
            Handler = handler;
            _rcon = rcon;
        }

        [SlashCommand("players", "Displays all the players currently online.")]
        public async Task UserRconCommand()
        {
            await _rcon.SendRconCommandAsync(Context, "players");
        }

        #region Help
        [RequireOwner]
        [SlashCommand("help", "Displays all the available commands.")]
        public async Task HelpRconCommand()
        {
            await _rcon.SendRconCommandAsync(Context, "help");
        }
        #endregion Help

        #region RCON
        [RequireOwner]
        [SlashCommand("rcon", "Sends an RCON command.")]
        public async Task RconCommand(string command)
        {
            await _rcon.SendRconCommandAsync(Context, command);
        }
        #endregion

        #region Teleport
        [RequireRole("Zomboider")]
        [SlashCommand("teleportto", "Teleports a player to another player.")]
        public async Task TeleportToSlashRconCommand(string username)
        {
            await _rcon.SendRconCommandAsync(Context, $"teleport \"{Context.User.Username}\" \"{username}\"");
        }

        [RequireRole("Zomboider")]
        [UserCommand("TeleportTo")]
        public async Task TeleportToUserRconCommand(IUser user)
        {
            await _rcon.SendRconCommandAsync(Context, $"teleport \"{Context.User.Username}\" \"{user.Username}\"");
        }
        #endregion Teleport

        #region GodMod
        [RequireOwner]
        [SlashCommand("godmod", "Make a player invincible. Use: /godmode \"username\".")]
        public async Task GodModRconCommand(string user)
        {
            await _rcon.SendRconCommandAsync(Context, $"godmod {user}");
        }

        [RequireOwner]
        [UserCommand("GodMod")]
        public async Task GodModUserRconCommand(IUser user)
        {
            await _rcon.SendRconCommandAsync(Context, $"godmod {user.Username}");
        }
        #endregion GodMod

        [RequireOwner]
        [SlashCommand("addvehicle", "Spawn a vehicle. ex: /addvehicle \"Base.VanAmbulance\" \"user or x,y,z\".")]
        public async Task AddVehicleRconCommand(string vehicle, string location)
        {
            await _rcon.SendRconCommandAsync(Context, $"addvehicle {vehicle} {location}");
        }
    }
}