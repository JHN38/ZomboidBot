#if FALSE
using Discord;
using Discord.Interactions;
using ServiceBot.Services;

namespace Bot.Modules
{
    // interation modules must be public and inherit from an IInterationModuleBase
    public class ExampleCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService? Commands { get; set; }
        public CommandHandler? Handler { get; set; }

        // constructor injection is also a valid way to access the dependecies
        public ExampleCommands(CommandHandler handler)
        {
            Handler = handler;
        }

        // our first /command!
        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question)
        {
            // create a list of possible replies
            var replies = new List<string>
            {
                // add our possible replies
                "yes",
                "no",
                "maybe",
                "hazzzzy...."
            };

            // get the answer
            var answer = replies[new Random().Next(replies.Count - 1)];

            // reply with the answer
            await RespondAsync($"You asked: [**{question}**], and your answer is: [**{answer}**]");
        }

        // Slash Commands are declared using the [SlashCommand], you need to provide a name and a description, both following the Discord guidelines
        [SlashCommand("ping", "Recieve a pong")]
        // By setting the DefaultPermission to false, you can disable the command by default. No one can use the command until you give them permission
        [DefaultPermission(false)]
        public async Task Ping()
        {
            await RespondAsync("pong");
        }

        // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
        // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
        // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

        // [Summary] lets you customize the name and the description of a parameter
        [SlashCommand("echo", "Repeat the input")]
        public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
        {
            await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));
        }

        // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
        [Group("test_group", "This is a command group")]
        public class GroupExample : InteractionModuleBase<SocketInteractionContext>
        {
            public enum ExampleEnum
            {
                First,
                Second,
                Third,
                Fourth
            }
            // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
            // choice option
            [SlashCommand("choice_example", "Enums create choices")]
            public async Task ChoiceExample(ExampleEnum input)
            {
                await RespondAsync(input.ToString());
            }
        }

        // User Commands can only have one parameter, which must be a type of SocketUser
        [UserCommand("SayHello")]
        public async Task SayHello(IUser user)
        {
            // Works
            var components = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithPlaceholder("Select an option")
                    .WithCustomId("menu-1")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOption(new SelectMenuOptionBuilder()
                        .WithEmote(new Emoji("🤖"))
                        .WithValue("cat")
                        .WithDescription("This is a cat.")
                        .WithLabel("Cat")
                        .WithDefault(true))
                    .AddOption(new SelectMenuOptionBuilder()
                        .WithEmote(new Emoji("🐶"))
                        .WithValue("dog")
                        .WithDescription("This is a dog.")
                        .WithLabel("Dog")));

            //// Errors out
            //var components = new ComponentBuilder()
            //    .WithSelectMenu(new SelectMenuBuilder()
            //        .AddOption(new SelectMenuOptionBuilder()
            //            .WithEmote(new Emoji("🤖"))
            //            .WithValue("ValueA")
            //            .WithDescription("DescriptionA")
            //            .WithLabel("LabelA")
            //            .WithDefault(true)
            //        )
            //    );

            await RespondAsync($"Hello, {user.Mention}", components: components.Build());
        }

        // Message Commands can only have one parameter, which must be a type of SocketMessage
        [MessageCommand("Delete")]
        [RequireOwner]
        public async Task DeleteMesage(IMessage message)
        {
            await message.DeleteAsync();
            await RespondAsync("Deleted message.");
        }

        // Use [ComponentInteraction] to handle message component interactions. Message component interaction with the matching customId will be executed.
        // Alternatively, you can create a wild card pattern using the '*' character. Interaction Service will perform a lazy regex search and capture the matching strings.
        // You can then access these capture groups from the method parameters, in the order they were captured. Using the wild card pattern, you can cherry pick component interactions.
        [ComponentInteraction("musicSelect:*,*")]
        public async Task ButtonPress(string id, string name)
        {
            
            // ...
            await RespondAsync($"Playing song: {name}/{id}");
        }

        // Select Menu interactions, contain ids of the menu options that were selected by the user. You can access the option ids from the method parameters.
        // You can also use the wild card pattern with Select Menus, in that case, the wild card captures will be passed on to the method first, followed by the option ids.
        [ComponentInteraction("roleSelect")]
        public async Task RoleSelect(params string[] selections)
        {
            await RespondAsync(string.Join(", ", selections));
        }
    }
}
#endif